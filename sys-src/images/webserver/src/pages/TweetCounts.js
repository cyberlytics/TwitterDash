import React from "react";
import styles from "../styles/Home.module.css";
import TweetCountsChart from "../components/tweet_count_chart";
import Navigation from "../components/Navigation";
import {TextField} from "@mui/material";
import TimeIntervalPicker from "../components/TimeIntervalPicker";

export default class TweetCounts extends React.Component {
    constructor(props) {
        super(props);
        let now = new Date();
        let one_week_ago = new Date(Date.now() - (1000 * 60 * 60 * 24 * 7))
        this.onKeyDownInput = this.onKeyDownInput.bind(this);
        this.state = {
            selected_hashtag: null,
            start_date: one_week_ago,
            end_date: now
        }

        this.handleNewDate = this.handleNewDate.bind(this);
    }

    handleNewDate(key, date) {
        this.setState({[key]: date});
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
                        <TimeIntervalPicker start_date={this.state.start_date} end_date={this.state.end_date} handleNewDate={this.handleNewDate}></TimeIntervalPicker>
                        <div className={"TextFieldWrapper"}>
                            <TextField
                                className={"TextFieldMUI"}
                                required
                                label="Trend"
                                onKeyDown={this.onKeyDownInput}
                            />
                        </div>
                        <div id="tweet_counts_chart">
                            <TweetCountsChart hashtag={this.state.selected_hashtag} start_date={this.state.start_date} end_date={this.state.end_date}></TweetCountsChart>
                        </div>
                    </div>
                </main>
            </div>
        )
    }
}