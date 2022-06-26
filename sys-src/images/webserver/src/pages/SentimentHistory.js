import React, { useEffect, useState, Fragment } from "react";
import styles from "../styles/Home.module.css";
import Link from "next/link";
import TweetCountsChart from "../components/tweet_count_chart";
import CountrySelection from "../components/CountrySelection";
import Navigation from "../components/Navigation";
import SentimentHistoryChart from "../components/SentimentHistoryChart";

export default class SentimentHistory extends React.Component {
    constructor() {
        super();
        this.onKeyDownInput = this.onKeyDownInput.bind(this);
        this.onCountrySelectChange = this.onSelectChange.bind(this, "country");
        this.state = {
            country: "Germany",
            selected_hashtag: null
        }
    }

    onSelectChange(key, e) {
        this.setState({[key]: e.target.value});
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
                        <CountrySelection onChange={this.onCountrySelectChange}></CountrySelection>
                        <div className="selection">
                            <span>hashtag: </span>
                            <input type="text" onKeyDown={this.onKeyDownInput}></input>
                        </div>
                        <div id="tweet_counts_chart">
                            <SentimentHistoryChart trendName={this.state.selected_hashtag} country={this.state.country}></SentimentHistoryChart>
                        </div>
                    </div>
                </main>
            </div>
        )
    }
}