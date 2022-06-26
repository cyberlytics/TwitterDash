import React, { Fragment } from "react";
import _ from "lodash";

export default class Selection extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            options: null
        }
    }

    updateOptions(options) {
        let processed_options = options.map((obj, index) => {
            return (
                <Fragment key={index}>
                    <option key={index} value={obj}>{obj}</option>
                </Fragment>
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
        return (
            <div className="selection">
                <span>{this.props.label} </span>
                <select defaultValue={this.props.defaultValue} onChange={this.props.onChange}>
                    { this.state.options }
                </select>
            </div>
        );
    }
}