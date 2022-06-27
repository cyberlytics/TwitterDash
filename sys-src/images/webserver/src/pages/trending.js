import React, { useEffect, useState, Fragment } from "react";
import styles from "../styles/Home.module.css";
import Link from "next/link";
import Trends from "../components/Trends";
import Selection from "../components/Selection";
import CountrySelection from "../components/CountrySelection";
import LimitSelection from "../components/LimitSelection"
import Navigation from "../components/Navigation";

export default class Trending extends React.Component {
    constructor(props) {
        super(props);
        this.onLimitSelectChange = this.onSelectChange.bind(this, "num_results");
        this.onCountrySelectChange = this.onSelectChange.bind(this, "country");
        this.state = {
            num_results: "10",
            country: "Germany",
        }
    }

    onSelectChange(key, e) {
        this.setState({[key]: e.target.value});
    }

    render() {
        return (
            <div className={styles.container}>
                <main className={styles.main}>
                    <Navigation active={"Trending Now"}></Navigation>
                    <div className="content">
                        <CountrySelection onChange={this.onCountrySelectChange}></CountrySelection>
                        <LimitSelection onChange={this.onLimitSelectChange} defaultValue={this.state.num_results}></LimitSelection>
                        <Trends num_results={this.state.num_results} country={this.state.country}></Trends>
                    </div>
                </main>
            </div>
        )
    }
}