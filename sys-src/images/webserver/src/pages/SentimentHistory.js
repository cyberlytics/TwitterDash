import React from "react";
import styles from "../styles/Home.module.css";
import Navigation from "../components/Navigation";
import SentimentHistoryChart from "../components/SentimentHistoryChart";
import {Autocomplete} from "@mui/material";
import {TextField} from "@mui/material";
import TimeIntervalPicker from "../components/TimeIntervalPicker";

const TYPING_DONE_DELAY = 250;

export default class SentimentHistory extends React.Component {
    constructor(props) {
        super(props);
        let now = new Date();
        let one_week_ago = new Date(Date.now() - (1000 * 60 * 60 * 24 * 7))
        this.state = {
            selected_hashtag: null,
            listOfOptions : [],
            start_date: one_week_ago,
            end_date: now
        }

        this.onKeyDownInput = this.onKeyDownInput.bind(this);
        this.delayDataRetrieval = this.delayDataRetrieval.bind(this);
        this.handleNewDate = this.handleNewDate.bind(this);

        this.timerIds = []
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
                    {label: obj, id:index}
                )
            });
            this.setState({listOfOptions})
        });
    }

    delayDataRetrieval(event) {
        this.clearAllTimers();
        if(event.key != "Enter" && event.key != "Escape") {
            this.timerIds.push(setTimeout(this.fetchData.bind(this), TYPING_DONE_DELAY, event.target.value));
        }
    }

    onKeyDownInput(e) {
        if (e.key === 'Enter') {
            this.setState({
                selected_hashtag: e.target.value
            });
        }
    }

    render() {
        return (
            <div className={styles.container}>
                <main className={styles.main}>
                    <Navigation active={"Sentiment History"}></Navigation>
                    <div className="content">
                        <TimeIntervalPicker start_date={this.state.start_date} end_date={this.state.end_date} handleNewDate={this.handleNewDate}></TimeIntervalPicker>
                        <Autocomplete
                            className={"autocomplete"}
                            autoHighlight
                            disablePortal
                            id="autocomplete_sentiment"
                            options={this.state.listOfOptions}
                            sx={{ width: 300 }}
                            renderInput={(params) => <TextField onChange={this.delayDataRetrieval} onKeyDown={this.onKeyDownInput} {...params} label="Trend" />}
                        />
                        <div id="tweet_counts_chart">
                            <SentimentHistoryChart trendName={this.state.selected_hashtag} start_date={this.state.start_date} end_date={this.state.end_date}></SentimentHistoryChart>
                        </div>
                    </div>
                </main>
            </div>
        )
    }
}