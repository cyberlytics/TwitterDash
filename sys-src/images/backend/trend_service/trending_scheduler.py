import schedule
import time
import twitter_api_caller

# Basti elevated Account
# Tokens für v1.1 ...
consumer_key_v1 = "97N3G5aEuk26CWyymmrJsp5Tn"
consumer_secret_v1 = "5lkdRGTtbIxrIeAIQOSZlOfcl1krXqky9i1zHCpkXLc2a2r0kt"
access_token_v1 = "1370306915062923269-O6Iu4FcoFcxxVsEK2SwYUYqGZn8ZYU"
access_token_secret_v1 = "CibkjBgAbvODvw67SUFeK2KsfdzrUU0eBHt7MAABpIdmU"

# Madin Account
# Tokens für v2
consumer_key_v2 = "3BPNKnwhdb2bOmkJPM3Nsr2xM"
consumer_secret_v2 = "U3v5EPJ1CyXG0Ow8VghsDVoue1iwwPhEbWntIvFDxUYcO1Eo4K"
access_token_v2 = "1524784534667112448-w4anzTJy16Fhz1k2PQD5Yw0lxy4E6y"
access_token_secret_v2 = "EbKcoxxLNhY8ZRilIDbuJ8BkN3n2ZNWVFY8n7brYBLWst"
bearer_token_v2 = "AAAAAAAAAAAAAAAAAAAAAMMVcgEAAAAAxbjzChVlAondghdM0sdOIeprL%2BU%3DK8r0YIP97sF9ViShCW0hqTjlkA4xowzva2FRI5de430tKPsESy"


if __name__ == "__main__":

    twitter_Caller = twitter_api_caller.Twitter_API_Caller(
        consumer_key_v1,
        consumer_secret_v1,
        access_token_v1,
        access_token_secret_v1,
        consumer_key_v2,
        consumer_secret_v2,
        access_token_v2,
        access_token_secret_v2,
        bearer_token_v2,
    )

    print(twitter_Caller.getTrending())

    # # Jede 15min werden die Trending Hashtags abgefragt und in der Datenbank abgespeichert
    # schedule.every(15).minutes.do(twitter_Caller.getTrending)
    # while True:
    #     schedule.run_pending()
    #     time.sleep(60 * 5)  # Check every minute
