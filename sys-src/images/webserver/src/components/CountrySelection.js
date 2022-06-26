import React, { Fragment } from "react";
import {Autocomplete, TextField} from "@mui/material";

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
            let availableCountries = data.map((obj, index) => {
                return {label: obj, id:index}
            });
            this.setState({availableCountries})
        });
    }

    componentDidMount() {
        this.fetchData();
    }

    render() {
        return (
            <Autocomplete
                className={"autocomplete"}
                onChange={this.props.onChange}
                autoHighlight
                disablePortal
                id="autocomplete_country"
                options={this.state.availableCountries}
                sx={{ width: 300 }}
                renderInput={(params) => <TextField {...params} label="Country" />}
            />
        );
    }
}