import React from "react";
import _ from "lodash";
import {FormControl, InputLabel, MenuItem, Select} from "@mui/material";

export default class Selection extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            options: null,
            value: this.props.defaultValue
        }
        this.handleChange = this.handleChange.bind(this);
    }

    handleChange(e) {
        this.setState({value: e.target.value});
        this.props.onChange(e);
    }

    updateOptions(options) {
        let processed_options = options.map((obj, index) => {
            return (
                <MenuItem key={index} value={obj}>{obj}</MenuItem>
            )
        });
        this.setState({options: processed_options});
    }


    componentDidMount() {
        if (this.props.raw_options != null) {
            this.updateOptions(this.props.raw_options);
        }
    }


    componentDidUpdate(prevProps) {
        if (!_.isEqual(this.props, prevProps)) {
            this.updateOptions(this.props.raw_options)
        }
    }

    render() {
        if (this.state.options == null) { return (<></>); }
        return (
            <div className={"SelectionWrapper"}>
                <FormControl className={"Selection"}>
                    <InputLabel>{this.props.label}</InputLabel>
                    <Select
                        value={this.state.value}
                        onChange={this.handleChange}
                        label={this.props.label}
                    >
                        { this.state.options }
                    </Select>
                </FormControl>
            </div>
        );
    }
}