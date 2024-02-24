# Ethnessa Relay Plugin (Discord Bridge)

The **Ethnessa Relay Plugin** seamlessly bridges Terraria servers with Discord, providing an engaging experience for players and server administrators. By relaying key server events to a Discord channel, it fosters a connected and interactive community environment. The plugin not only enhances communication but also sets the stage for future cross-server capabilities, powered by the EthnessaAPI.

## 🌟 Features

- **Discord Integration:** Automatically relays join, leave, chat, and death notifications from Terraria to Discord.
- **Advanced Player Authentication:** Enables players to link their Terraria server accounts with Discord, facilitating secure and interactive command execution.
- **Flexible Configuration:** Offers detailed settings for message relay preferences, bot tokens, and channel management.
- **Live Server Status:** Dynamically updates the bot's status to reflect real-time player counts and server activity.
- **Enhanced Command Support:** Introduces Discord commands for direct server management and player engagement.

## 📋 Requirements

- Terraria Server (with EthnessaAPI Version 1.0)
- A Discord Bot Token (configured with message and channel management permissions)

## ⚙️ Installation

### Discord Bot Setup
1. Create a Discord bot on the [Discord Developer Portal](https://discord.com/developers/applications).
2. Note the bot token and adjust the necessary permissions for message and channel management.

### MongoDB Configuration
Ensure MongoDB is installed and operational, noting your connection details.

### Plugin Installation
- Download the Ethnessa Relay plugin.
- Place it into your server's `ServerPlugins` directory.

### Configuration
- On first run, a `data/relay.json` configuration file will be generated.
- Edit this file to include your Discord bot token, preferred channel IDs, and other settings.

## 🚀 Usage

With the plugin configured, start your Terraria server. Ethnessa Relay will begin its magic, connecting your server events with your Discord community. Account linking and command interactions provide a seamless experience between Terraria gameplay and Discord social engagement.

## 📚 Commands

Ethnessa Relay enriches your Discord with Terraria-specific commands:

- **/playersonline**: Displays the current number of players on the server.
- **/login**: Facilitates account linking between Discord and Terraria.
- **/execute**: Allows authenticated users to run commands on the Terraria server.

## 💡 Contributing

Contributions are warmly welcomed! Whether you're fixing bugs, adding features, or improving documentation, your efforts are valuable to us. Feel free to submit pull requests or open issues on our GitHub repository.

---

Elevate your Terraria server with Ethnessa Relay — Where worlds meet, communities connect.
