import React from "react";
import Selection from "./Selection";

export default class LimitSelection extends React.Component {
    constructor(props) {
        super(props);
        this.limits = ["5","10","25","50"];
    }

    render() {
        return (
            <Selection label="Limit" onChange={this.props.onChange} defaultValue={this.props.defaultValue} raw_options={this.limits}></Selection>
        );
    }
}