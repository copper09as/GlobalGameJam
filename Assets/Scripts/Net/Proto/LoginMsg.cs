using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
public class MsgRegister:MsgBase
{
    public MsgRegister()
    {
        protoName = "MsgRegister";
    }
    public string id = "";
    public string pw = "";
    public int result = 0;
}
public class MsgLogin : MsgBase
{ 
    public MsgLogin()
    {
        protoName = "MsgLogin";
    }
    public string id = "";
    public string pw = "";
    public int result = 0;
}
public class MsgKick : MsgBase
{
    public MsgKick()
    {
        protoName = "MsgKick";
    }
    public int reason = 0;

}

