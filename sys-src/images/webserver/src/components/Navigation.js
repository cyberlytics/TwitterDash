import React, { Fragment } from "react";
import Link from "next/link";
import _ from "lodash";

export default class Navigation extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            nav: null
        }

        this.links = [
            {
                label: "Twitter Dash",
                href: "/"
            },
            {
                label: "Trending",
                href: "/trending"
            },
            {
                label: "Tweet Counts",
                href: "/TweetCounts"
            },
            {
                label: "Sentiment History",
                href: "/SentimentHistory"
            },
            {
                label: "Trend History",
                href: "/TrendHistory"
            }
        ];
    }

    changeActive() {
        let nav = this.links.map((obj, index) => {
            if (this.props.active != obj.label) {
                return (
                    <Fragment key={index}>
                        <Link key={index} href={obj.href}><a>{obj.label}</a></Link>
                    </Fragment>
                );
            }
            else {
                return (
                    <Fragment key={index}>
                        <Link key={index} href={obj.href}><a className="active">{obj.label}</a></Link>
                    </Fragment>
                );
            }

        });
        this.setState({nav});
    }

    componentDidMount() {
        if (this.props.active != null) {
            this.changeActive();
        }
    }


    componentDidUpdate(prevProps) {
        if (!_.isEqual(this.props, prevProps)) {
            this.changeActive()
        }
    }

    render() {
        return (
            <div className="topnav">
                {this.state.nav}
            </div>
        );
    }
}