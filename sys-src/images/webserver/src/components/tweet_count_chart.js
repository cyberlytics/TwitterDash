import React, { useEffect, useState, Fragment } from "react";
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
            text: 'Line chart with dummy data',
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
        console.log("props TweetCountsChart")
        console.log(props)
        super(props);
        this.state = {
            hashtag: props.hashtag,
            data: null,
            loading: false,
        }
    }

    componentDidUpdate(prevProps) {
        if (this.props.hashtag != prevProps.hashtag) {
            this.setState({
                hashtag: this.props.hashtag
            })

            if (this.props.hashtag != null) {
                let query = 'api/get_tweet_counts?' + new URLSearchParams({
                    hashtag: this.props.hashtag
                });
                let fetch_promise = fetch(query);
                let json_promise = fetch_promise.then((res) => res.json())
                json_promise.then((data) => this.setState({
                    data: data
                }));
            }
        }
    }

    render() {
        if (!this.state.hashtag) return <p>No hashtag selected</p>
        if (!this.state.data) return <p>No data!</p>

        let dataSlice = this.state.data.slice(0, Math.floor(this.state.data.length/15))

        let displayData = dataSlice

        let labels = displayData.map((obj, index) => {
            return (
                obj.end
            );
        });
        let tweetCounts = displayData.map((obj, index) => {
            return (
                obj.tweet_count
            );
        });

        const chartData = {
            labels: labels,
            datasets: [
                {
                    label: this.state.hashtag,
                    data: tweetCounts,
                    borderColor: 'rgb(255, 99, 132)',
                    backgroundColor: 'rgba(255, 99, 132, 0.5)',
                }
            ]
        }

        //console.log(data)

        return (<Line options={options} data={chartData} />);
    }

}