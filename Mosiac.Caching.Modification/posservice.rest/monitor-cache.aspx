<%@ Page Language="C#" AutoEventWireup="true" Inherits="MonitorCachePage" Codebehind="monitor-cache.aspx.cs" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title runat="server">Monitor Page</title>
    <link href="bootstrap.min.css" rel="stylesheet" />
	<link rel="stylesheet" type="text/css" href="css.css" />
</head>
<body>
    <form id="form1" runat="server">
    <div>
		<h3><asp:Literal ID="headerLtl" runat="server" /></h3>
		<hr />
<%--        <div class="row">
            <div class="col-xs-3 table-bordered text-right">Col1</div>
            <div class="col-xs-3 table-bordered">Col2</div>
            <div class="col-xs-3 table-bordered">Col3</div>
            <div class="col-xs-3 table-bordered">Col4</div>
        </div>
        <div class="row">
            <div class="col-xs-3 col-xs-offset-3 table-bordered">Col2</div>
            <div class="col-xs-3 table-bordered">Col3</div>
            <div class="col-xs-3 table-bordered">Col4</div>
        </div>
        <div class="row">
            <div class="col-xs-3 col-xs-offset-6 table-bordered">Col3</div>
            <div class="col-xs-3 table-bordered">Col4</div>
        </div>--%>

<%--		<h4 style="background-color:#CCCCCC;">Cache</h4>--%>
		<asp:PlaceHolder runat="server" ID="cacheHolder" />


<%--		<h4 style="background-color:#CCCCCC;">Service Cache</h4>
		<asp:PlaceHolder runat="server" ID="serviceCacheHolder" />
		
		<h4 style="background-color:#CCCCCC;">Store Cache</h4>
		<asp:PlaceHolder runat="server" ID="storeCacheHolder" />
    
		<h4 style="background-color:#CCCCCC;">Terminal Cache</h4>
		<asp:PlaceHolder runat="server" ID="terminalCacheHolder" />--%>

    </div>
    </form>
</body>
</html>
