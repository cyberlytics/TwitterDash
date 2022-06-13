import React, { useEffect, useState, Fragment } from "react";

export default class Trends extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            data: null
        }
    }

    fetchData() {
        let query = 'api/get_hashtag_trends?' + new URLSearchParams({
            num_results: this.props.num_results
        });
        let fetch_promise = fetch(query);
        let json_promise = fetch_promise.then((res) => res.json())
        json_promise.then((data) => {
            let results = data.map((obj, index) => {
                return (
                    <Fragment key={index + 1}>
                        <tr key={index + 1}>
                            <th key={0}>{index + 1}</th>
                            <th key={1}>{obj.name}</th>
                            <th key={2}>{obj.tweet_volume}</th>
                        </tr>
                    </Fragment>
                )
            })
            this.setState({data: results})
        });
    }

    componentDidMount() {
        this.fetchData();
    }

    componentDidUpdate(prevProps) {
        if (this.props.num_results != prevProps.num_results) {
            this.setState({
                num_results: this.props.num_results
            })
            this.fetchData();
        }
    }

    render() {
        if (!this.state.data) return <p>No data!</p>
        return (
            <table>
                <tbody>
                <tr key={0}>
                    <th key={0}>Rank</th>
                    <th key={1}>Hashtag</th>
                    <th key={2}>Tweet Volume</th>
                </tr>
                { this.state.data }
                </tbody>
            </table>
        );
    }
}