import React from "react";
import {LocalizationProvider} from "@mui/x-date-pickers";
import {AdapterDateFns} from "@mui/x-date-pickers/AdapterDateFns";
import {DateTimePicker} from "@mui/x-date-pickers/DateTimePicker";
import {TextField} from "@mui/material";

export default class TimeIntervalPicker extends React.Component {
    constructor(props) {
        super(props);

        this.state = {
            start_date: this.props.start_date,
            end_date: this.props.end_date
        }
        this.handleNewStartDate = this.handleNewDate.bind(this, "start_date");
        this.handleNewEndDate = this.handleNewDate.bind(this, "end_date");
    }

    handleNewDate(key, date) {
        this.props.handleNewDate(key, date);
        this.setState({[key]: date});
    }

    render() {
        return (
            <LocalizationProvider dateAdapter={AdapterDateFns}>
                <div className={"Wrapper"}>
                    <DateTimePicker
                        className={"TimePicker"}
                        label="Start Date"
                        value={this.state.start_date}
                        onChange={this.handleNewStartDate}
                        renderInput={(params) => <TextField {...params} fullWidth />}
                    />
                </div>
                <div className={"Wrapper"}>
                    <DateTimePicker
                        className={"TimePicker"}
                        label="End Date"
                        value={this.state.end_date}
                        onChange={this.handleNewEndDate}
                        renderInput={(params) => <TextField {...params} fullWidth/>}
                    />
                </div>
            </LocalizationProvider>
        );
    }
}