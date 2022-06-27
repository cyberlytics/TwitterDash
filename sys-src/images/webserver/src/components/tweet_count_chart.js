import React from "react";
import _ from "lodash";
import {
    Chart as ChartJS,
    TimeScale,
    CategoryScale,
    LinearScale,
    PointElement,
    LineElement,
    Title,
    Tooltip,
    Legend
} from 'chart.js';
import 'chartjs-adapter-moment';
import { Line } from 'react-chartjs-2';
ChartJS.register(
    TimeScale,
    CategoryScale,
    LinearScale,
    PointElement,
    LineElement,
    Title,
    Tooltip,
    Legend
);

export const options = {
    responsive: true,
    type: 'line',
    plugins: {
        legend: {
            position: 'top',
        },
        title: {
            display: true,
            text: 'Tweet Counts',
        },
    },
    scales: {
        x: {
            type: 'time',
            time: {
                unit: 'day'
            },
            ticks: {
                sampleSize: 2,
                maxTicksLimit: 7
            }
        },
        y: {
            beginAtZero: true
        }
    },
    elements: {
        line: {
            tension: 0 // disables bezier curves
        }
    }
};

export default class TweetCountsChart extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            hashtag: props.hashtag,
            data: null,
            loading: false,
        }
    }

    fetchData() {
        let query = 'api/get_tweet_counts?' + new URLSearchParams({
            hashtag: this.props.hashtag,
            start_date: this.props.start_date,
            end_date: this.props.end_date
        });
        let fetch_promise = fetch(query);
        let json_promise = fetch_promise.then((res) => res.json())
        json_promise.then((data) => this.setState({
            data: data
        }));
    }

    componentDidUpdate(prevProps) {
        if (!_.isEqual(this.props, prevProps)) {
            if (this.props.hashtag != null && this.props.start_date != null && this.props.end_date != null) {
                this.fetchData();
            }
        }
    }

    render() {
        if (!this.props.hashtag) return <p>Please enter your search term ...</p>
        if (!this.state.data) return <p>Hang on ...</p>

        let displayData = this.state.data;

        let labels = displayData.map((obj, index) => {
            return (
                new Date(obj.datetime.seconds * 1000)
            );
        });
        let tweetCounts = displayData.map((obj, index) => {
            return (
                obj.count
            );
        });

        const chartData = {
            labels: labels,
            datasets: [
                {
                    label: this.props.hashtag,
                    data: tweetCounts,
                    borderColor: 'rgb(255, 99, 132)',
                    backgroundColor: 'rgba(255, 99, 132, 0.5)',
                }
            ]
        }

        return (<Line options={options} data={chartData} />);
    }

}