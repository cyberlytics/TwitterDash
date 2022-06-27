import React from "react";
import styles from "../styles/Home.module.css";
import TweetCountsChart from "../components/tweet_count_chart";
import Navigation from "../components/Navigation";
import {TextField} from "@mui/material";
import { DateTimePicker } from '@mui/x-date-pickers/DateTimePicker';
import {LocalizationProvider} from "@mui/x-date-pickers";
import { AdapterDateFns } from '@mui/x-date-pickers/AdapterDateFns';

export default class TweetCounts extends React.Component {
    constructor(props) {
        super(props);
        let now = new Date();
        let one_week_ago = new Date(Date.now() - (1000 * 60 * 60 * 24 * 7))
        this.onKeyDownInput = this.onKeyDownInput.bind(this);
        this.state = {
            country: "Germany",
            selected_hashtag: null,
            start_date: one_week_ago,
            end_date: now
        }
        this.handleNewStartDate = this.handleNewDate.bind(this, "start_date");
        this.handleNewEndDate = this.handleNewDate.bind(this, "end_date");
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
                        <LocalizationProvider dateAdapter={AdapterDateFns}>
                            <div className={"Wrapper"}>
                                <DateTimePicker
                                    className={"TimePicker"}
                                    label="Start Date"
                                    value={this.state.start_date}
                                    onChange={this.handleNewStartDate}
                                    renderInput={(params) => <TextField {...params} fullWidth />}
                                />
                            </div>
                            <div className={"Wrapper"}>
                                <DateTimePicker
                                    className={"TimePicker"}
                                    label="End Date"
                                    value={this.state.end_date}
                                    onChange={this.handleNewEndDate}
                                    renderInput={(params) => <TextField {...params} fullWidth/>}
                                />
                            </div>
                        </LocalizationProvider>
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