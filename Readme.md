# Mirage (Mobile Industrial Robot Manager)

## Description
Mirage is a powerful tool designed to manage objects within Mobile Industrial Robots (MiR). It allows users to configure and control various robot components, including:
- Maps
- Site settings
- Sounds
- Dashboards
- Footprints
- Missions

### Robots Supported
Mirage currently supports:
- **MiR 250** (tested on version 3.5.5)
- Likely compatible with **MiR version 3 and above**, with API version 2 and above

## Installation
To install Mirage, download the latest release from the [GitHub Releases](https://github.com/Bluedragonplayz2/Mirage/releases) page.

## Usage
Mirage provides several commands to manage robot configurations and data:

---
### Clear Dashboards
Removes all dashboards from the specified robots.
```
Usage: cleardashboard <robot(s)>
Alias: cdash
```
**Arguments:**
- `<robot(s)>` - The target robot(s) to clear dashboards from. Format options:
  - `[fleet name*]`
  - `[fleet name*]/[robot name*]`
  - `[ip*]:[port]:[username*]:[password*]`
  - `"all"` (all robots in config, separated by `,`)

---
### Clear Footprints
Removes all footprints from the specified robots.
```
Usage: clearfootprint <robot(s)>
Alias: cf
```
**Arguments:**
- `<robot(s)>` - The target robot(s) to clear footprints from. Format options:
  - `[fleet name*]`
  - `[fleet name*]/[robot name*]`
  - `[ip*]:[port]:[username*]:[password*]`
  - `"all"` (all robots in config, separated by `,`)

---
### Clear Missions
Removes all missions from the specified robots at a given site.
```
Usage: clearmission <robot(s)> <site name>
Alias: cm
```
**Arguments:**
- `<robot(s)>` - The target robot(s) to clear missions from. Format options:
  - `[fleet name*]`
  - `[fleet name*]/[robot name*]`
  - `[ip*]:[port]:[username*]:[password*]`
  - `"all"` (all robots in config, separated by `,`)
- `<site name>` - The site name to clear missions from.

---
### Sync Site Files
Transfers site files from a source robot to multiple target robots.
```
Usage: syncsite <source robot> <target robots> <site name>
Alias: ss
```
**Arguments:**
- `<source robot>` - The robot that contains the site to be transferred. Format options:
  - `[fleet name*]/[robot name*]`
  - `[ip*]:[port]:[username*]:[password*]`
- `<target robots>` - The robots that will receive the site. Format options:
  - `[fleet name*]`
  - `[fleet name*]/[robot name*]`
  - `[ip*]:[port]:[username*]:[password*]`
  - `"all"` (all robots in config, separated by `,`)
- `<site name>` - The name of the site to be transferred.

---
## Configuration
Mirage is built for Windows x64 and runs on .NET 8. It is self-contained, meaning no additional dependencies are required.

## Contributing
For contribution guidelines, please refer to the [Contributing Guide](https://github.com/Bluedragonplayz2/Mirage/blob/main/CONTRIBUTING.md) on GitHub.

## License
For licensing details, please refer to the [License](https://github.com/Bluedragonplayz2/Mirage/blob/main/LICENSE) file on GitHub.

## Contact
For support or inquiries, please open an issue on the [GitHub Issues](https://github.com/Bluedragonplayz2/Mirage/issues) page.

