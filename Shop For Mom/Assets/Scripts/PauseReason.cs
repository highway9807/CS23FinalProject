/* PauseReason.cs holds an enum for valid pause reasons. this reduces
 * Spelling-based errors (e.g. calling request pause with "Inventory" and 
 * release pause )
 *
 * Note: though enums may cause some (tiny in our case) overhead, they're useful
 *       to standardize pausing. If we simply allowed calling programs to input
 *       strings, then mismatched strings/typos called in RequestPause() and
 *       ReleasePause() may cause nasty bugs.
*/
public enum PauseReason
{
    None        = 0, // basically a null value for this enum
    PauseMenu   = 1,

    /*... Other reasons go here */

}
