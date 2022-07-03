import React from "react";
import styles from "../styles/Home.module.css";
import Navigation from "../components/Navigation";
import SentimentHistoryChart from "../components/SentimentHistoryChart";
import {Autocomplete} from "@mui/material";
import {TextField} from "@mui/material";
import TimeIntervalPicker from "../components/TimeIntervalPicker";
import GranularitySelection from "../components/GranularitySelection";
import { withRouter } from 'next/router'

const TYPING_DONE_DELAY = 250;

export default withRouter(class SentimentHistory extends React.Component {
    constructor(props) {
        super(props);
        this.maxDate = new Date();
        let one_week_ago = new Date(Date.now() - (1000 * 60 * 60 * 24 * 7))
        this.state = {
            selected_hashtag: null,
            selected_hashtag_graph: null,
            listOfOptions : [],
            start_date: one_week_ago,
            end_date: this.maxDate,
            granularity: "hour"
        }

        this.delayDataRetrieval = this.delayDataRetrieval.bind(this);
        this.handleNewDate = this.handleNewDate.bind(this);
        this.onGranularitySelectChange = this.onSelectChange.bind(this, "granularity");
        this.autoCompleteHandleChange = this.autoCompleteHandleChange.bind(this);

        this.timerIds = []
    }

    componentDidMount() {
        if (this.props.router.query.trendName) {
            this.setState({
                selected_hashtag: {label: this.props.router.query.trendName, id:0},
                selected_hashtag_graph: this.props.router.query.trendName
            })
        }
    }

    autoCompleteHandleChange(event, value) {
        if (value == null) {
            this.setState({
                listOfOptions: [],
                selected_hashtag: null,
                selected_hashtag_graph: null
            })
        }
        else {
            this.setState({
                selected_hashtag: value,
                selected_hashtag_graph: value.label
            })
        }
    }

    onSelectChange(key, e) {
        this.setState({[key]: e.target.value});
    }

    handleNewDate(key, date) {
        this.setState({[key]: date});
    }

    clearAllTimers() {
        //copy wegen async
        for (let timerId of [...this.timerIds]) {
            clearTimeout(timerId);
        }
        this.timerIds = []
    }

    fetchData(searchQuery) {
        let query = 'api/GetTrendsWithAvailableSentiment?' + new URLSearchParams({
            query: searchQuery,
            limit: 5
        });
        let fetch_promise = fetch(query);
        let json_promise = fetch_promise.then((res) => res.json())
        json_promise.then((data) => {
            let listOfOptions = data.map((obj, index) => {
                return (
                    {label: obj, id:index+1}
                )
            });
            this.setState({listOfOptions})
        });
    }

    delayDataRetrieval(event) {
        this.setState({selected_hashtag: null})
        this.clearAllTimers();
        if (event.target.value != "") {
            this.timerIds.push(setTimeout(this.fetchData.bind(this), TYPING_DONE_DELAY, event.target.value));
        }
        else {
            this.setState({listOfOptions: []});
        }
    }

    render() {
        return (
            <div className={styles.container}>
                <main className={styles.main}>
                    <Navigation active={"Sentiment History"}></Navigation>
                    <div className="content">
                        <TimeIntervalPicker start_date={this.state.start_date} end_date={this.state.end_date} minDate={null} maxDate={this.maxDate} handleNewDate={this.handleNewDate}></TimeIntervalPicker>
                        <GranularitySelection onChange={this.onGranularitySelectChange} defaultValue={this.state.granularity} excludeMinute={true}></GranularitySelection>
                        <Autocomplete
                            className={"autocomplete"}
                            onChange={this.autoCompleteHandleChange}
                            autoHighlight
                            disablePortal
                            id="autocomplete_sentiment"
                            options={this.state.listOfOptions}
                            sx={{ width: 300 }}
                            value={this.state.selected_hashtag}
                            renderInput={(params) => <TextField onChange={this.delayDataRetrieval} {...params} label="Trend" />}
                        />
                        <div id="tweet_counts_chart">
                            <SentimentHistoryChart trendName={this.state.selected_hashtag_graph} start_date={this.state.start_date} end_date={this.state.end_date} granularity={this.state.granularity}></SentimentHistoryChart>
                        </div>
                    </div>
                </main>
            </div>
        )
    }
})