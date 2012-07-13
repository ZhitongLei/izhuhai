<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="register.aspx.cs" Inherits="iZhuHai.register" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>爱珠海</title>
    <link rel="stylesheet" type="text/css" href="iZhuHai.css" />
    <script type="text/javascript" language="javascript" src="./js/jquery-1.5.1.js"></script>
    <script type="text/javascript" language="javascript" src="./js/iZhuHai.js"></script>
  
</head>
<body>
    <form id="form1" runat="server">
    <div class="title">
    </div>
    <div class="apply">
        <form action="WebForm1.aspx" method="post" id="applyForm" class="applyForm">
        <label>
            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;邮箱：<input type="text" id="email" runat="server" class="textInput" /><br />
        </label>
        <label>
            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;姓名：<input type="text" id="names" runat="server" class="textInput" value="" /><br />
        </label>
        <label>
            <p class="introduce">
                个人介绍：</p>
            <p style="float: left;">
                <textarea id="introduce" runat="server"></textarea><br />
            </p>
        </label>
        <input id="applySub" type="submit" value="申请" />
        </form>
    </div>
    <div id="alertDiv">
        <p id="alertEmail" class="alerts">
            请填写正确的邮箱地址。</p>
        <p id="alertName" class="alerts">
            请填写您的真实姓名。</p>
        <p id="alertIntroduce" class="alerts">
            请填写你的学校和专业等个人信息。</p>
    </div>
    </form>
</body>
</html>
