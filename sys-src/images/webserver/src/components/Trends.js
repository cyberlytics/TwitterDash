import React, { useEffect, useState, Fragment } from "react";
import { DataGrid, GridColDef, GridValueGetterParams } from '@mui/x-data-grid';
import {Button} from "@mui/material";

const _ = require("lodash");

const cols = [
    { field: "placement", headerName: "Placement", flex: 1},
    { field: "trendName", headerName: "Trend", sortable: false, flex: 1},
    { field: "sentiment", headerName: "Sentiment", flex: 1},
    { field: "tweetVolume24", headerName: "Tweet Volume 24h", flex: 1}
]


export default class Trends extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            data: null
        }

        this.processData = this.processData.bind(this);
        this.fetchData = this.fetchData.bind(this);
    }

    async processData(data) {
        let map_promises = data.map(async (obj, index) => {
            let sentiment = await this.fetchSentiment(obj.name);
            sentiment = sentiment.toFixed(2);
            return (
                {
                    id: index,
                    placement: obj.placement,
                    trendName: obj.name,
                    sentiment,
                    tweetVolume24: obj.tweetVolume24
                }
            );
        });
        let processedData = await Promise.all(map_promises);
        this.setState({data: processedData});
    }

    async fetchSentiment(trendName) {
        let query = 'api/GetCurrentSentiment?' + new URLSearchParams({ trendName });
        let fetch_promise = fetch(query);
        return fetch_promise.then((res) => res.json())
    }

    fetchData() {
        let query = 'api/get_hashtag_trends?' + new URLSearchParams({
            num_results: this.props.num_results,
            country: this.props.country
        });
        let fetch_promise = fetch(query);
        let json_promise = fetch_promise.then((res) => res.json())
        json_promise.then((data) => {
            this.processData(data);
        });
    }

    componentDidMount() {
        this.fetchData();
    }

    componentDidUpdate(prevProps) {
        if (!_.isEqual(this.props, prevProps)) {
            this.fetchData();
        }
    }

    render() {
        if (!this.state.data) return <p>No data!</p>
        return (
            <div className={"tableWrapper"}>
                <DataGrid
                    rows={this.state.data}
                    columns={cols}
                    pageSize={10}
                    rowsPerPageOptions={[10]}
                    autoHeight
                />
            </div>
        );
    }
}