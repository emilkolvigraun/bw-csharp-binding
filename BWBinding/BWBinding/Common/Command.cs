using System.ComponentModel;

namespace BWBinding.Common
{
    public enum Command
    {
        [DescriptionAttribute("publ")] PUBLISH,
        [DescriptionAttribute("pers")] PERSIST,
        [DescriptionAttribute("subs")] SUBSCRIBE,
        [DescriptionAttribute("list")] LIST,
        [DescriptionAttribute("quer")] QUERY,
        [DescriptionAttribute("tsub")] TAP_SUBSCRIBE,
        [DescriptionAttribute("tque")] TAP_QUERY,
        [DescriptionAttribute("putd")] PUT_DOT,
        [DescriptionAttribute("pute")] PUT_ENTITY,
        [DescriptionAttribute("putc")] PUT_CHAIN,
        [DescriptionAttribute("makd")] MAKE_DOT,
        [DescriptionAttribute("make")] MAKE_ENTITY,
        [DescriptionAttribute("makc")] MAKE_CHAIN,
        [DescriptionAttribute("bldc")] BUILD_CHAIN,
        [DescriptionAttribute("adpd")] ADD_PREF_DOT,
        [DescriptionAttribute("adpc")] ADD_PREF_CHAIN,
        [DescriptionAttribute("dlpc")] DEL_PREF_CHAIN,
        [DescriptionAttribute("sete")] SET_ENTITY,
        [DescriptionAttribute("helo")] HELLO,
        [DescriptionAttribute("resp")] RESPONSE,
        [DescriptionAttribute("rslt")] RESULT
    }
}
