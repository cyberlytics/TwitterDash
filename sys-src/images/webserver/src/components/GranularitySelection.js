import React from "react";
import Selection from "./Selection";

export default class GranularitySelection extends React.Component {
    constructor(props) {
        super(props);
        this.granularities = props.excludeMinute ? ["hour", "day"] : ["minute", "hour", "day"];
    }

    render() {
        return (
            <Selection label="Granularity" onChange={this.props.onChange} defaultValue={this.props.defaultValue} raw_options={this.granularities}></Selection>
        );
    }
}