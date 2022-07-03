import React, { Fragment } from "react";
import {Autocomplete, TextField} from "@mui/material";
import Selection from "./Selection";

export default class CountrySelection extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            availableCountries: []
        }
    }

    fetchData() {
        let query = 'api/GetAvailableCountries';
        let fetch_promise = fetch(query);
        let json_promise = fetch_promise.then((res) => res.json())
        json_promise.then((data) => {
            this.setState({availableCountries: data})
        });
    }

    componentDidMount() {
        this.fetchData();
    }

    render() {
        if (this.state.availableCountries.length == 0) { return (<></>); }
        return (
            <Selection label="Country" onChange={this.props.onChange} defaultValue={this.props.defaultValue} raw_options={this.state.availableCountries}></Selection>
        );
    }
}