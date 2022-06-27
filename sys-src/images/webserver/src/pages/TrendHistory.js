import React from "react";
import styles from "../styles/Home.module.css";
import Navigation from "../components/Navigation";
import TrendHistoryChart from "../components/TrendHistoryChart";
import {Autocomplete} from "@mui/material";
import {TextField} from "@mui/material";
import TimeIntervalPicker from "../components/TimeIntervalPicker";
import CountrySelection from "../components/CountrySelection";

const TYPING_DONE_DELAY = 250;

export default class TrendHistory extends React.Component {
    constructor(props) {
        super(props);
        this.maxDate = new Date();
        let one_week_ago = new Date(Date.now() - (1000 * 60 * 60 * 24 * 7))
        this.state = {
            selected_hashtag: null,
            country: "Germany",
            listOfOptions : [],
            start_date: one_week_ago,
            end_date: this.maxDate
        }

        this.onCountrySelectChange = this.onSelectChange.bind(this, "country");
        this.onKeyDownInput = this.onKeyDownInput.bind(this);
        this.delayDataRetrieval = this.delayDataRetrieval.bind(this);
        this.handleNewDate = this.handleNewDate.bind(this);

        this.timerIds = []
    }

    onSelectChange(key, e) {
        console.log(e.target.value);
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

    //Sollte im Interface noch eine äquivalente Methode für die Trends ergängt werden, dann änndern!
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
                    <Navigation active={"Trend History"}></Navigation>
                    <div className="content">
                        <CountrySelection onChange={this.onCountrySelectChange}></CountrySelection>
                        <TimeIntervalPicker start_date={this.state.start_date} end_date={this.state.end_date} minDate={null} maxDate={this.maxDate} handleNewDate={this.handleNewDate}></TimeIntervalPicker>
                        <Autocomplete
                            className={"autocomplete"}
                            autoHighlight
                            disablePortal
                            id="autocomplete_Trend"
                            options={this.state.listOfOptions}
                            sx={{ width: 300 }}
                            renderInput={(params) => <TextField onChange={this.delayDataRetrieval} onKeyDown={this.onKeyDownInput} {...params} label="Trend" />}
                        />
                        <div id="tweet_counts_chart">
                            <TrendHistoryChart trendName={this.state.selected_hashtag} start_date={this.state.start_date} end_date={this.state.end_date} country={this.state.country}></TrendHistoryChart>
                        </div>
                    </div>
                </main>
            </div>
        )
    }
}