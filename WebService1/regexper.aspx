<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="regexper.aspx.cs" Inherits="WebService1.llamada" culture="auto" meta:resourcekey="PageResource1" uiculture="auto" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <link rel='shortcut icon' href='images/ic_tune_black_24dp.png' type='image/png' />
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <link rel="stylesheet" href="//code.jquery.com/ui/1.11.0/themes/smoothness/jquery-ui.css" />
    <link rel="stylesheet" type="text/css" href="CSS/Style.css" />    
    <script type="text/javascript" src="Libreria/Jquery 2.1.1.js"></script>   
    <script type="text/javascript" src="Libreria/Raphael.js"></script>
    <script type="text/javascript" src="Libreria/jquery-ui.js"></script>
    <script type="text/javascript" src="Libreria/Code.js"></script>
    <title><asp:Literal runat="server" Text="<%$ Resources : PageResource1.Title%>" /></title>
</head>
<body>
    <header>
        <div id="cabeza">
            <img src="images/ulpgc.png" alt="logo_ulpgc" height="100" width="360"/>
            <h1 id="header"><asp:Literal ID="Literal1" runat="server" Text="<%$ Resources : PageResource1.Title%>" /></h1>
        </div>
    </header>
    <section id="content">
        <div id="container">
            <form id="form1" runat="server">
                <div id="inputs">
                    <asp:ScriptManager ID="ScriptManager1" runat="server">
                        <Services>
                            <asp:ServiceReference Path="~/Service1.asmx" />
                        </Services>
                    </asp:ScriptManager>
                    <asp:TextBox ID="txtData" runat="server" placeholder="<%$Resources: regex%>" autofocus="autofocus"></asp:TextBox>
                    <br/>
                        <input id="Button1" type="button"  value="<asp:Literal ID='Literal3' runat='server' Text='<%$ Resources : b1%>' />"/>
                    <br/>           
                </div>
                <span id="same"></span>
                <div id="Match">                        
                </div>
            </form>
        </div>
        <div id="expresion">
            </div>
         <div id="result">             
        </div>
    </section>
</body>
</html>
