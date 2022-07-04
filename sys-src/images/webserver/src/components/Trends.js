import React, { useEffect, useState, Fragment } from "react";
import { DataGrid, GridColDef, GridValueGetterParams } from '@mui/x-data-grid';
import {Button} from "@mui/material";
import { withRouter } from 'next/router'

const _ = require("lodash");

const cols = [
    { field: "placement", headerName: "Placement", flex: 1, cellClassName: "clickableCell"},
    { field: "trendName", headerName: "Trend", sortable: false, flex: 1},
    { field: "sentiment", headerName: "Sentiment", flex: 1, cellClassName: "clickableCell"},
    { field: "tweetVolume24", headerName: "Tweet Volume 24h", flex: 1, cellClassName: "clickableCell"}
]


export default withRouter(class Trends extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            data: null
        }

        this.processData = this.processData.bind(this);
        this.fetchData = this.fetchData.bind(this);
        this.onCellClick = this.onCellClick.bind(this);
    }

    onCellClick(params) {
        switch(params.field) {
            case "sentiment":
                this.props.router.push({
                    pathname: "/SentimentHistory",
                    query: {trendName: params.row.trendName}
                });
                break;
            case "placement":
                this.props.router.push({
                    pathname: "/TrendHistory",
                    query: {
                        trendName: params.row.trendName,
                        country: this.props.country
                    }
                });
                break;
            case "tweetVolume24":
                this.props.router.push({
                    pathname: "/TweetCounts",
                    query: {
                        trendName: params.row.trendName
                    }
                });
                break;
        }
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
        if (!this.state.data) return <p>Hang on ...</p>
        return (
            <div className={"tableWrapper"}>
                <DataGrid
                    rows={this.state.data}
                    columns={cols}
                    pageSize={8}
                    rowsPerPageOptions={[8]}
                    onCellClick={this.onCellClick}
                    autoHeight
                    disableSelectionOnClick
                />
            </div>
        );
    }
})