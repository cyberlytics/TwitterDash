import React, { Fragment } from "react";
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
        return (
            <Selection label="country: " onChange={this.props.onChange} defaultValue="Germany" raw_options={this.state.availableCountries}></Selection>
        );
    }
}