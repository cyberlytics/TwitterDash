import React, { useEffect, useState, Fragment } from "react";

export default function Trends(props) {
    const [data, setData] = useState(null)
    const [isLoading, setLoading] = useState(false)

    useEffect(() => {
        setLoading(true)
        fetch('api/get_hashtag_trends')
            .then((res) => res.json())
            .then((data) => {
                setData(data)
                setLoading(false)
            })
    }, [])

    if (isLoading) return <p>Loading...</p>
    if (!data) return <p>No profile data</p>

    let results = data.map((obj, index) => {
        return (
            <Fragment>
                <tr key={index + 1}>
                    <th key={0}>{index + 1}</th>
                    <th key={1}>{obj.name}</th>
                    <th key={2}>{obj.tweet_volume}</th>
                </tr>
            </Fragment>
        )
    })
    return (
        <>
            <table>
                <tbody>
                <tr key={0}>
                    <th key={0}>Rank</th>
                    <th key={1}>Hashtag</th>
                    <th key={2}>Tweet Volume</th>
                </tr>
                { results }
                </tbody>
            </table>
        </>
    );
}