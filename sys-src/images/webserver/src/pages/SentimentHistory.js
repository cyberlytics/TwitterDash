import React, { useEffect, useState, Fragment } from "react";
import styles from "../styles/Home.module.css";
import Link from "next/link";
import TweetCountsChart from "../components/tweet_count_chart";
import CountrySelection from "../components/CountrySelection";
import Navigation from "../components/Navigation";
import SentimentHistoryChart from "../components/SentimentHistoryChart";
import {Autocomplete} from "@mui/material";
import {TextField} from "@mui/material";

const TYPING_DONE_DELAY = 250;

export default class SentimentHistory extends React.Component {
    constructor() {
        super();
        this.onChangeTest = this.onChangeTest.bind(this);
        this.onKeyDownInput = this.onKeyDownInput.bind(this);
        this.onCountrySelectChange = this.onSelectChange.bind(this, "country");
        this.delayDataRetrieval = this.delayDataRetrieval.bind(this);
        this.state = {
            country: "Germany",
            selected_hashtag: null,
            listOfOptions : []
        }

        this.InputProps = { classes: { inputTypeSearch: 'search-input', input: 'input' } }
        this.timerIds = []
    }



    clearAllTimers() {
        //copy wegen async
        for (let timerId of [...this.timerIds]) {
            clearTimeout(timerId);
        }
        this.timerIds = []
    }

    fetchData(searchQuery) {
        let query = 'api/GetAvailableSentimentTrends?' + new URLSearchParams({
            query: searchQuery,
            limit: 5,
            country: this.state.country
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

    onChangeTest(e) {
        console.log("change!");
    }

    onSelectChange(key, e) {
        this.setState({[key]: e.target.textContent});
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
                        <Autocomplete
                            className={"autocomplete"}
                            autoHighlight
                            disablePortal
                            id="autocomplete_sentiment"
                            options={this.state.listOfOptions}
                            sx={{ width: 300 }}
                            renderInput={(params) => <TextField onChange={this.delayDataRetrieval} onKeyDown={this.onKeyDownInput} {...params} label="Trend" />}
                        />
                        <CountrySelection onChange={this.onCountrySelectChange}></CountrySelection>
                        <div id="tweet_counts_chart">
                            <SentimentHistoryChart trendName={this.state.selected_hashtag} country={this.state.country}></SentimentHistoryChart>
                        </div>
                    </div>
                </main>
            </div>
        )
    }
}