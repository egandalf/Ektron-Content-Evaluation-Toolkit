<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ContentEvaluation.aspx.cs" Inherits="handlers_devhelp_ContentEvaluation" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Content Evaluation Tool</title>
    <style>
        * {
            font-family: sans-serif;
            font-size: 11pt;
        }

        html, body {
            margin: 0;
            padding: 0;
            height: 100%;
        }

        h1 {
            font-size: 130%;
        }

        h2 {
            font-size: 120%;
        }

        section {
            position: relative;
            padding-top: 30px;
            background: #00529b;
            z-index: 1;
            border: 1px solid #000;
        }

            section div.container {
                overflow-y: auto;
                max-height: 500px;
                z-index: 500;
            }

        table {
            border-spacing: 0;
            width: 100%;
        }

        thead {
            border-bottom: 1px solid #000;
        }

        th, td {
            padding: 5px 10px;
        }

            td span {
                display: block;
            }

        th {
            height: 0;
            line-height: 0;
            padding-top: 0;
            padding-bottom: 0;
            color: transparent;
            border: 0;
            white-space: nowrap;
            z-index: 501;
            overflow: hidden;
        }

            th div {
                position: absolute;
                color: #fff;
                padding: 5px 10px;
                margin-top: -33px;
                line-height: normal;
                overflow: visible;
                z-index: 1000;
            }

        tr {
            background: #fff;
        }

            tr.alternate {
                background: #efefef;
            }

        td.center {
            text-align: center;
        }

        .form-container {
            margin: 5px;
            overflow: hidden;
        }

        .field-collection {
            display: inline-block;
            border: 1px solid #ccc;
            padding: 10px;
            width: 1240px;
        }

        .btn /*button*/ {
            background-color: #00529b;
            border: 0;
            padding: 10px;
            color: #fff;
        }

        .form-field {
            padding: 5px;
            height: 25px;
            display: table-cell;
            text-align: center;
            vertical-align: middle;
        }

            .form-field label {
                display: inline-block;
                color: #555;
                width: 200px;
                line-height: 25px;
                vertical-align: middle;
            }

            .form-field input {
                height: 25px;
                vertical-align: middle;
            }

            .form-field span.value-output {
                border: 2px #f1c4c4 dashed;
                color: #777;
                display: inline-block;
                padding: 3px;
            }

        .manual {
            color: green;
        }

        .script {
            color: red;
        }

        .visible {
            display: block;
        }

        .invisible {
            display: none;
        }

        .padded {
            padding:10px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:Panel ID="uxDataCollection" runat="server" DefaultButton="uxGetResults" CssClass="form-container">
                <div>
                    <button id="toggleInstructionsBtn" class="btn" runat="server">Hide Instructions</button>
                </div>
                <div id="instructions" class="visible" runat="server">
                    <h1>Content Estimation Tool</h1>
                    <p>This tool is provided free of charge and only to provide an estimation of scale for the content to be moved.</p>
                    <p>
                        In other words:
                    </p>
                    <ul>
                        <li><em>DO</em> use this to get an idea for what you can handle yourself versus what should probably go toward script- or tool-based transfer.</li>
                        <li><em>DO NOT</em> use this tool as a guarantee for the level of effort required to move your content.</li>
                    </ul>
                    <p>
                        As an additional note, please keep in mind that this tool does not take into account:
                    </p>
                    <ul>
                        <li>Content freeze windows or the need for multiple or ongoing updates to content.</li>
                        <li>Links within content that will need to be updated.</li>
                        <li>Images or other downloadable assets within content that will need to be moved.</li>
                        <li>Any content not directly managed by the CMS.</li>
                        <li>Additional time required to move and re-configure visitor Forms.</li>
                        <li>Development time to support the rendering of content to the site.</li>
                        <li>Content may need to be mapped to a new, different set of properties due to redesign or other content strategy efforts.</li>
                    </ul>
                    <p>
                        An explanation of the fields below:
                    </p>
                    <ol>
                        <li><b>Minutes Per Item</b> - The baseline number of estimated minutes it takes to manually duplicate a 'simple' or unstructured piece of content. Should include time for standard content properties such as SEO metadata.</li>
                        <li><b>Seconds per Field</b> - For content with multiple fields, the estimated number of seconds it takes to copy any given additional field.</li>
                        <li><b>Percentage for QA</b> - The baseline percentage of content items to be used for quality checks. This value will be given a multiple if the content type is subject to an approval chain (assumes the content will need additional validation). For content that 'Sometimes' needs approval, this is multiplied by 1.5. For content types always requiring approval, it's multiplied by 2.</li>
                        <li><b>QA Minutes per Item</b> - Number of minutes estimated to be spent undergoing QA for a single item.</li>
                        <li><b>Manual Effort</b> - The estimated number of hours a team is willing to commit to for manual reentry of content in the new system.</li>
                        <li><b>High Velocity Threshold</b> - No high-velocity content should be moved manually. This helps us determine what you consider to be high or low for your company. Choose the option you think implies content is changing too frequently to be moved manually.</li>
                    </ol>
                    <p>Enjoy!</p>
                </div>
                <div class="field-collection">
                    <h2>Estimate Configuration</h2>
                    <div class="form-field">
                        <asp:Label ID="uxMinutesPerItemLabel" runat="server" AssociatedControlID="uxMinutesPerItem">Minutes Per Item (Base):</asp:Label>
                        <asp:TextBox ID="uxMinutesPerItem" runat="server" TextMode="Number" Text="3" />
                    </div>
                    <div class="form-field">
                        <asp:Label ID="uxSecondsPerFieldLabel" runat="server" AssociatedControlID="uxSecondsPerField">Seconds per Field:</asp:Label>
                        <asp:TextBox ID="uxSecondsPerField" runat="server" TextMode="Number" Text="15" />
                    </div>
                    <div class="form-field">
                        <asp:Label ID="uxPercentQALabel" runat="server" AssociatedControlID="uxPercentQA">Percentage for QA:</asp:Label>
                        <asp:TextBox ID="uxPercentQA" runat="server" TextMode="Range" min="1" max="100" value="10" ClientIDMode="Static" /><span class="value-output" id="uxSliderValue"></span>
                    </div>
                    <div class="form-field">
                        <asp:Label ID="uxQAMinutesLabel" runat="server" AssociatedControlID="uxQAMinutes">QA Minutes per Item</asp:Label>
                        <asp:TextBox ID="uxQAMinutes" runat="server" TextMode="Number" Text="1" />
                    </div>
                    <div class="form-field">
                        <asp:Label ID="uxInternalTimeLabel" runat="server" AssociatedControlID="uxInternalTime">Manual Effort (Hours):</asp:Label>
                        <asp:TextBox ID="uxInternalTime" runat="server" TextMode="Number" Text="40" />
                    </div>
                    <div class="form-field">
                        <asp:Label ID="uxVelocityThresholdLabel" runat="server" AssociatedControlID="uxVelocityThreshold">High Velocity Threshold</asp:Label>
                        <asp:DropDownList ID="uxVelocityThreshold" runat="server">
                            <asp:ListItem Selected="True" Text="Once Per Month" Value="monthly"></asp:ListItem>
                            <asp:ListItem Text="Twice Per Month" Value="bimonthly"></asp:ListItem>
                            <asp:ListItem Text="Once Per Week" Value="weekly"></asp:ListItem>
                            <asp:ListItem Text="Twice Per Week" Value="biweekly"></asp:ListItem>
                            <asp:ListItem Text="Once Per Day" Value="daily"></asp:ListItem>
                        </asp:DropDownList>
                    </div>
                    <asp:Button ID="uxGetResults" runat="server" OnClick="uxGetResults_Click" CssClass="btn" Text="Get Results" />
                </div>
                <script type="text/javascript">
                    var setValueText = function (input, output, prefix, suffix) {
                        output.innerHTML = prefix + input.value + suffix;
                    }

                    var collectValue = function (sliderID, valueID) {
                        var self = this;
                        self.elem = this.document.getElementById(sliderID);
                        self.display = this.document.getElementById(valueID);

                        setValueText(self.elem, self.display, "", "%");

                        self.elem.oninput = function () {
                            setValueText(self.elem, self.display, "", "%");
                        }
                    }

                    window.onload = collectValue("uxPercentQA", "uxSliderValue");
                </script>

            </asp:Panel>
            <asp:ListView ID="uxContentData" runat="server" ItemPlaceholderID="itemPlaceholder">
                <LayoutTemplate>
                    <section>
                        <div class="container">
                            <table class="types">
                                <thead>
                                    <tr class="header">
                                        <th>Content Type Name
                                            <div>Content Type Name</div>
                                        </th>
                                        <th>Content Type ID
                                            <div>Content Type ID</div>
                                        </th>
                                        <th>Fields
                                            <div>Fields</div>
                                        </th>
                                        <th>Field Count
                                            <div>Field Count</div>
                                        </th>
                                        <th>Content Volume
                                            <div>Content Volume</div>
                                        </th>
                                        <th>Annual Velocity
                                            <div>Annual Velocity</div>
                                        </th>
                                        <th>Folders
                                            <div>Folders</div>
                                        </th>
                                        <th>Subject to Approval
                                            <div>Subject To Approval</div>
                                        </th>
                                    </tr>
                                </thead>
                                <tbody>
                                    <asp:PlaceHolder ID="itemPlaceholder" runat="server" />
                                </tbody>
                            </table>
                        </div>
                    </section>
                </LayoutTemplate>
                <ItemTemplate>
                    <tr>
                        <td><%#((ContentTypeInformation)Container.DataItem).Name %></td>
                        <td class="center"><%#((ContentTypeInformation)Container.DataItem).ID %></td>
                        <td><%#((ContentTypeInformation)Container.DataItem).Fields %></td>
                        <td class="center"><%#((ContentTypeInformation)Container.DataItem).FieldCount %></td>
                        <td class="center"><%#((ContentTypeInformation)Container.DataItem).Volume %></td>
                        <td class="center"><b><%#((ContentTypeInformation)Container.DataItem).Velocity %></b> /day</td>
                        <td><%#((ContentTypeInformation)Container.DataItem).Folders %></td>
                        <td><%#((ContentTypeInformation)Container.DataItem).RequiresApproval %></td>
                    </tr>
                </ItemTemplate>
                <AlternatingItemTemplate>
                    <tr class="alternate">
                        <td><%#((ContentTypeInformation)Container.DataItem).Name %></td>
                        <td class="center"><%#((ContentTypeInformation)Container.DataItem).ID %></td>
                        <td><%#((ContentTypeInformation)Container.DataItem).Fields %></td>
                        <td class="center"><%#((ContentTypeInformation)Container.DataItem).FieldCount %></td>
                        <td class="center"><%#((ContentTypeInformation)Container.DataItem).Volume %></td>
                        <td class="center"><b><%#((ContentTypeInformation)Container.DataItem).Velocity %></b> /day</td>
                        <td><%#((ContentTypeInformation)Container.DataItem).Folders %></td>
                        <td><%#((ContentTypeInformation)Container.DataItem).RequiresApproval %></td>
                    </tr>
                </AlternatingItemTemplate>
            </asp:ListView>

            <asp:ListView ID="uxValues" runat="server" ItemPlaceholderID="itemPlaceholder">
                <LayoutTemplate>
                    <div class="padded">
                        <h2>Evaluation</h2>
                        <p>
                            Content types are listed for either <em class="manual">manual</em> or <em class="script">scripted</em> effort. In order for a type to qualify for manual effort, it must fit within both the hours of manual effort allowed <em>and below the velocity threshold</em>.
                        </p>
                        <p>
                            For this reason, the <em class="manual">manual</em> items may be well short of the allowed manual effort.
                        </p>
                        <ul>
                            <asp:PlaceHolder ID="itemPlaceholder" runat="server" />
                        </ul>
                    </div>
                </LayoutTemplate>
                <ItemTemplate>
                    <li>
                        <%#Eval("Name") %>:
                        <%--<b><%#Eval("Value") %> man-hours</b>--%>
                        <span class="<%#Eval("Class") %>">
                            <%#Eval("Message") %>
                        </span>
                    </li>
                </ItemTemplate>
            </asp:ListView>
        </div>
    </form>
    <script src="https://code.jquery.com/jquery-2.2.0.min.js"></script>
    <script type="text/javascript">
        $(function () {
            $("#toggleInstructionsBtn").click(function (e) {
                e.preventDefault();
                $instructions = $("#instructions");
                $btn = $(this);
                $instructions.fadeToggle(function () {
                    if ($instructions.is(':visible')) {
                        $btn.text("Hide Instructions");
                    } else {
                        $btn.text("Show Instructions");
                    }
                });
            });
        });
    </script>
</body>
</html>
