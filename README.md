# Ektron Content Evaluation Toolkit

This tool is designed to give you some idea for the level of effort required (and the approach you likely should take) when moving content out of Ektron and into Episerver. While it is somewhat general, it is not intended to be used to determine efforts to move into other platforms and should not be used in lieu of or in place of expert evaluations of the project.

Note that this is in no way a gaurantee and the mathematics involved make some assumptions. It attempts to address ideas such as content complexity and content value through additions or multiplications of the time involved to move the content manually. Much of this is configurable within the tool.

The Excel files are identical with the exception of how the totals and end-results are computed. The version with macros is able to limit the items flagged for manual effort by limiting them to the first N which fall below the indicated velocity threshold and within the desired manual effort window. The non-macros version is unable to limit the items to the desired threshold and, instead, will mark the calculated hours in RED if they total above the configured allowed effort.

The best option is to use the code provided. It provides similar calculations and determinations, but removes the manual effort of entering Smart Form types as well as guesswork around the velocity of the content. The code has only been tested against a site with around 4,000 items. If you have more than that, it may take some time (and even time out) when retrieving the data it needs to perform the evaluation. If so, a timeout will have to be set in the code behind. I confess this isn't my cleanest or most efficient code but it should work for many customers and can be tweaked by those with the know-how to make it better.

## Instructions from File

###Content Estimation Tool

This tool is provided free of charge and only to provide an estimation of scale for the content to be moved.

In other words:

* DO use this to get an idea for what you can handle yourself versus what should probably go toward script- or tool-based transfer.
* DO NOT use this tool as a guarantee for the level of effort required to move your content.

As an additional note, please keep in mind that this tool does not take into account:

* Content freeze windows or the need for multiple or ongoing updates to content.
* Links within content that will need to be updated.
* Images or other downloadable assets within content that will need to be moved.
* Any content not directly managed by the CMS.
* Additional time required to move and re-configure visitor Forms.
* Development time to support the rendering of content to the site.
* Content may need to be mapped to a new, different set of properties due to redesign or other content strategy efforts.

An explanation of the fields below:

1. **Minutes Per Item** - The baseline number of estimated minutes it takes to manually duplicate a 'simple' or unstructured piece of content. Should include time for standard content properties such as SEO metadata.
2. **Seconds per Field** - For content with multiple fields, the estimated number of seconds it takes to copy any given additional field.
3. **Percentage for QA** - The baseline percentage of content items to be used for quality checks. This value will be given a multiple if the content type is subject to an approval chain (assumes the content will need additional validation). For content that 'Sometimes' needs approval, this is multiplied by 1.5. For content types always requiring approval, it's multiplied by 2.
4. **QA Minutes per Item** - Number of minutes estimated to be spent undergoing QA for a single item.
5. **Manual Effort** - The estimated number of hours a team is willing to commit to for manual reentry of content in the new system.
6. **High Velocity Threshold** - No high-velocity content should be moved manually. This helps us determine what you consider to be high or low for your company. Choose the option you think implies content is changing too frequently to be moved manually.

Enjoy!
