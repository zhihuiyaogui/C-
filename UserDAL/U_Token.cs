
//------------------------------------------------------------------------------
// <auto-generated>
//     此代码已从模板生成。
//
//     手动更改此文件可能导致应用程序出现意外的行为。
//     如果重新生成代码，将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------


namespace UserDAL
{

using System;
    using System.Collections.Generic;
    
public partial class U_Token
{

    public long ID { get; set; }

    public string TokenID { get; set; }

    public Nullable<bool> IsDisabled { get; set; }

    public Nullable<System.DateTime> DisabledTime { get; set; }

    public Nullable<long> UserID { get; set; }

    public Nullable<int> UserFrom { get; set; }

    public Nullable<bool> Uniqueness { get; set; }

    public Nullable<System.DateTime> Expiration { get; set; }

    public string AesKey { get; set; }

    public string AesIv { get; set; }

    public Nullable<System.DateTime> CreateTime { get; set; }

}

}
