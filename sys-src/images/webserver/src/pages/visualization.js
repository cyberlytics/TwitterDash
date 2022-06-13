import React, { useEffect, useState, Fragment } from "react";
import styles from "../styles/Home.module.css";
import Link from "next/link";
import TweetCountsChart from "../components/tweet_count_chart";

export default class Visualization extends React.Component {
    constructor() {
        super();
        this.onSelectChange = this.onSelectChange.bind(this);
        this.state = {
            selected_hashtag: null
        }
    }

    onSelectChange(e) {
        this.setState({
            selected_hashtag: e.target.value
        });
    }

    render() {
        return (
            <div className={styles.container}>
                <main className={styles.main}>
                    <div className="topnav">
                        <Link href="/"><a>Twitter Dash</a></Link>
                        <Link href="/trending"><a>Trending</a></Link>
                        <Link href="/visualization"><a className="active">Visualization</a></Link>
                    </div>
                    <div className="content">
                        <div className="selection">
                            <span>hashtag: </span>
                            <select onChange={this.onSelectChange}>
                                <option disabled selected value> select hashtag </option>
                                <option value="BTS_Proof">#BTS_Proof</option>
                                <option value="YetToCome">#YetToCome</option>
                            </select>
                        </div>
                        <div id="tweet_counts_chart">
                            <TweetCountsChart hashtag={this.state.selected_hashtag}></TweetCountsChart>
                        </div>
                    </div>
                </main>
            </div>
        )
    }
}