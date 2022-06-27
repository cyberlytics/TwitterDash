import React, { Fragment } from "react";
import Selection from "./Selection";

export default class LimitSelection extends React.Component {
    constructor(props) {
        super(props);
        this.limits = ["5","10","25","50"];
    }

    render() {
        return (
            <Selection label="limit" onChange={this.props.onChange} defaultValue="5" raw_options={this.limits}></Selection>
        );
    }
}