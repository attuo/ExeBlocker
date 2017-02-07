# ExeBlocker

Blocks wanted exes from opening by using Windows registry.
When user tries to open the exe either warning popup appears or nothing stars.
The blocker doesn't need to be running while block is on. 

Currently a console application.

## Commands and user interface
 
Direct string[args] commands:
 * start <-- Will put the block on if it has been put to off.
 * add exename.exe <-- Adds exe to blocklist
 
### User interface:

    Welcome to ExeBlocker!
    Block is currently on.
    1) Add exe names you want to be blocked
    2) List all exes from blocklist
    3) Delete exe from blocklist
    4) Delete all exes from blocklist
    5) Start block
    6) Stop block
    7) Exit program

## Getting started

#### Option 1
Download the working exe and use it by opening it.
#### Option 2
Download the working exe, place it in some folder and start it with console. 
Set new environment variable for the path the ExeBlocker.exe exists

## Contributing

Feel free to contribute.

## Author

Atte Tuomisto - Personal project - attuo

## License

This project is licensed under the MIT License - see the LICENSE.md file for details
