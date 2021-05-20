Create Function dbo.FnSplit( @string varchar(4000))
Returns
@Result Table (value varchar(100))
As
Begin
    declare @len int, @loc int = 1
    While @loc <= len(@string) 
    Begin
        Set @len = CHARINDEX(',', @string, @loc) - @loc
        If @Len < 0 Set @Len = len(@string)
        Insert Into @Result Values (SUBSTRING(@string,@loc,@len))
        Set @loc = @loc + @len + 1
    End
    Return
End