import React, { useEffect, useState, Fragment } from "react";
import styles from "../styles/Home.module.css";
import Link from "next/link";
import TweetCountsChart from "../components/tweet_count_chart";
import CountrySelection from "../components/CountrySelection";
import Navigation from "../components/Navigation";

export default class TweetCounts extends React.Component {
    constructor(props) {
        super(props);
        this.onKeyDownInput = this.onKeyDownInput.bind(this);
        this.state = {
            country: "Germany",
            selected_hashtag: null
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
                    <Navigation active={"Tweet Counts"}></Navigation>
                    <div className="content">
                        <div className="selection">
                            <span>hashtag: </span>
                            <input type="text" onKeyDown={this.onKeyDownInput}></input>
                        </div>
                        <div id="tweet_counts_chart">
                            <TweetCountsChart hashtag={this.state.selected_hashtag} country={this.state.country}></TweetCountsChart>
                        </div>
                    </div>
                </main>
            </div>
        )
    }
}