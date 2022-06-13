import React, { useEffect, useState, Fragment } from "react";
import styles from "../styles/Home.module.css";
import Link from "next/link";
import Trends from "../components/Trends";

export default class Trending extends React.Component {
    constructor() {
        super();
        this.onSelectChange = this.onSelectChange.bind(this);
        this.state = {
            num_results: "50"
        }
    }

    onSelectChange(e) {
        this.setState({
            num_results: e.target.value
        });
    }

    render() {
        return (
            <div className={styles.container}>
                <main className={styles.main}>
                    <div className="topnav">
                        <Link href="/"><a>Twitter Dash</a></Link>
                        <Link href="/trending"><a className="active">Trending</a></Link>
                        <Link href="/visualization"><a>Visualization</a></Link>
                    </div>
                    <div>
                        <span>top n results: </span>
                        <select defaultValue="50" onChange={this.onSelectChange}>
                            <option value="5">5</option>
                            <option value="10">10</option>
                            <option value="25">25</option>
                            <option value="50">50</option>
                        </select>
                    </div>
                    <Trends num_results={this.state.num_results}></Trends>
                </main>
            </div>
        )
    }
}