import React from "react";
import styles from "../styles/Home.module.css";
import Trends from "../components/Trends";
import CountrySelection from "../components/CountrySelection";
import LimitSelection from "../components/LimitSelection"
import Navigation from "../components/Navigation";

export default class Trending extends React.Component {
    constructor(props) {
        super(props);
        this.onLimitSelectChange = this.onSelectChange.bind(this, "num_results");
        this.onCountrySelectChange = this.onSelectChange.bind(this, "country");
        this.state = {
            num_results: "5",
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
                        <div className={"contentRow"}>
                            <CountrySelection onChange={this.onCountrySelectChange} defaultValue={"Germany"}></CountrySelection>
                            <LimitSelection onChange={this.onLimitSelectChange} defaultValue={this.state.num_results}></LimitSelection>
                        </div>
                        <div className={"results"}>
                            <Trends num_results={this.state.num_results} country={this.state.country}></Trends>
                        </div>
                    </div>
                </main>
            </div>
        )
    }
}