# Cardano Monitor

This is a small tool I built to monitor the price of ADA, and notify me when certain conditions are met.

For example, I might want to know if the Cardano price is crashing / surging to help me make a well timed purchase.

Right now, the following features are supported:
 - Bittrex API markets for ADA, BTC, and USDT
 - Balance monitor
   Activates if you could reach a target balance of a given currency
 - Price monitor
   Activates if the price of a given currency (relative to another currency) passes above or below some threshold
 - Relative price monitor
   Activates if the price of a given currency changes by a certain threshold
 - Console notifications
 - Beep notifications

Features I might implement if there's an interest in the community:
 - Email notifications
 - SMS notifications via Twillio
 - Docker image for easy setup
 - Read balance from a wallet using the Daedalus API
 - Other exchanges
 - Other monitor types (Ideas welcome!)
 - Other currencies (Maybe make this general purpose?)

If I'm feeling really ambitious, I might add the ability to allow it to make trades in response to any of the monitors.

# Configuration
Configuration can be read in from a JSON file.  I'll document this more thoroughly soon.

# Support
If you'd like to support what I'm doing, you can donate to any of the following addresses:

## ADA
DdzFFzCqrhsdvtnMWNa1BwfYcP6euas5LwXbSJzS37f4nhagXH1aQizTCj9tjpHeF59GVB2p6CZVuHCQbP3ExadzZ4ApWxwSFqtu5iKZ

![ADA Donation Address](https://chart.googleapis.com/chart?cht=qr&chs=256x256&chl=DdzFFzCqrhsdvtnMWNa1BwfYcP6euas5LwXbSJzS37f4nhagXH1aQizTCj9tjpHeF59GVB2p6CZVuHCQbP3ExadzZ4ApWxwSFqtu5iKZ&chld=L)

## Bitcoin
[1BvpquVdFdJxJasBVufBrkPcfFVmf1Vg2g](bitcoin:1BvpquVdFdJxJasBVufBrkPcfFVmf1Vg2g?message=Cardano%20Monitor%20Donations)

![Bitcoin Donation Address](https://chart.googleapis.com/chart?cht=qr&chs=256x256&chl=bitcoin:1BvpquVdFdJxJasBVufBrkPcfFVmf1Vg2g?message=Cardano%2520Monitor%2520Donations&chld=L)