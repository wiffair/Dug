# Dug
DUG is a DNS lookup tool written in C#.  

Dug is loosely modeled on DIG, but currently supports only a subset of the options.

I plan on adding a *"Discover"* option, that will try to find any orphaned sub-domains.

## Usage
**Usage:** dug Name  
**Usage:** dug {[@DNS Server]} {[-options]} {[+options} Name  
***NB:** The order of the arguments is not important.*

|  Argument  | Description |
| ---------- | --- |
| @TargetServer | Where TargetServer is the IP address of the DNS server you would like to query. Multiple servers can be targeted in one query. |
| Name | The name or address you would like to resolve. |

|  Argument  | Description |
| ---------- | --- |
| -t QueryType | Where QueryType is one or more of A, AAAA, MX, NS, PTR or SOA |
|-o Output | Where Output is one or more of CSV, JSON, NOC |
|-F FileName |        Where Filename is the full path for the JSON output. (Only used with the -o JSON option) |
|-H | Only display the head information in the console |
|-q | Include the query information when displaying the output in the console |
|-V | Display verbose information.  When running a TRACE this will display the 'A' record lookups for the Name Servers |
|-v |  Display the version information and then stop |
|-h |  Display this help screen |

***NB:** The '-' options **ARE** case sensitive.*

|  Argument  | Description |
| ---------- | --- |
|+RDFlag |Set Recursive Lookup flag (default) |
|+NoRDFlag | Unset Recursive Lookup flag|
|+RECURSE | Same as +RDFlag |
|+NoRECURSE | Same as +NoRDFlag |
|+ADFlag | Set Authenticated Data flag (default) |
|+NoADFlag | Unset Auhenticated Data flag |
|+CDFlag | Set Checking Disabled flag |
|+NoCDFlag | Unset Checking Disabled flag (default) |
|+RAW | Display the raw packet bytes in the console output |
|+TRACE | Trace delegation down from root |
|+TEST |  Perform a lookup on the final result of a TRACE |
|+TimeOut | Set Timeout value in seconds (default is 5 seconds) |
|+Retry | Set the number of reties after a timeout (default is 4) |

***NB:** The '+' options are **NOT** case sensitive.*


|  Argument  | Description |
| ---------- | --- |
| Show License | Display the GNU3 License |
| Show Help | Display this help screen |

***NB:** The 'Show' options are **NOT** case sensitive.*

### Output Format:
There are three options for the output format:
* CSV:

    A subset of the gathered information displayed as csv with a header.
* JSON:

    JSON output of the full collected data. The data will either be displayed in the console window, or saved to a file if the -F flag has been used.
* NOC:

    Stops the display of the standard output - can be used with either the CSV or JSON options.


Unless otherwise specified, all source code hosted on this repository is licensed under the GPLv3 license. See the [LICENSE](https://github.com/wiffair/Dug?tab=GPL-3.0-1-ov-file#readme) file for more details.
