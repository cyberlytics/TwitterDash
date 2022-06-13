import Head from 'next/head'
import Link from 'next/link'
import Image from 'next/image'
import styles from '../styles/Home.module.css'
import Trends from '../components/Trends'
import TweetCountsChart from '../components/tweet_count_chart'
import React, { useEffect, useState, Fragment } from "react";

export default class Home extends React.Component {
    render() {
        return (
            <div className={styles.container}>
                <Head>
                    <title>Twitter Dash</title>
                    <meta name="description" content="Generated by create next app" />
                </Head>

                <main className={styles.main}>
                    <div className="topnav">
                        <Link href="/"><a className="active">Twitter Dash</a></Link>
                        <Link href="/trending"><a>Trending</a></Link>
                        <Link href="/visualization"><a>Visualization</a></Link>
                    </div>
                    <h1 className={styles.title}>
                        Twitter Dash
                    </h1>
                    <p>Introduction to Twitter Dash ...</p>
                    <p>Technical description of Twitter Dash ...</p>
                    <p>Usage description of Twitter Dash ...</p>
                    <p>Authors of Twitter Dash ...</p>

                </main>
            </div>
        )
    }

}
