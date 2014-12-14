#include "LevelParser.h"
#include <limits.h>

#define LTN_CHILD_COUNT 26

struct LevelTreeNode_
{
    const int Value;
    const LevelTreeNode_ *Children[LTN_CHILD_COUNT];
};

static int LtnGetIndex_ (char c)
{
    return c - 'A';
}

static int LtnGet_ (const LevelTreeNode_ *root, const char *value, size_t valueSize)
{
    int result;
    const LevelTreeNode_ *currentNode = root;

    while (currentNode)
    {
        result = currentNode->Value;
        if (valueSize <= 0 || !*value)
        {
            break;
        }

        auto idx = LtnGetIndex_ (*value);
        if (0 > idx || idx >= LTN_CHILD_COUNT)
        {
            return -2;
        }
        currentNode = currentNode->Children[idx];

        ++value;
        --valueSize;
    }

    if (valueSize > 0 && *value)
    {
        result = -1;
    }

    return result;
}

#pragma region Levels definition

static const LevelTreeNode_ LevelAll_ =
{
    INT_MIN,
    //      A        B        C        D        E        F        G        H        I        J        K        L        M        N        O        P        Q        R        S        T        U        V        W        X        Y        Z
    { nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr }
};
static const LevelTreeNode_ LevelAl_ =
{
    -1,
    //      A        B        C        D        E        F        G        H        I        J        K           L        M        N        O        P        Q        R        S        T        U        V        W        X        Y        Z
    { nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, &LevelAll_, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr }
};
static const LevelTreeNode_ LevelA_ =
{
    -1,
    //      A        B        C        D        E        F        G        H        I        J        K          L        M        N        O        P        Q        R        S        T        U        V        W        X        Y        Z
    { nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, &LevelAl_, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr }
};
static const LevelTreeNode_ LevelCritical_ =
{
    90000,
    //      A        B        C        D        E        F        G        H        I        J        K        L        M        N        O        P        Q        R        S        T        U        V        W        X        Y        Z
    { nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr }
};
static const LevelTreeNode_ LevelCritica_ =
{
    -1,
    //      A        B        C        D        E        F        G        H        I        J        K                L        M        N        O        P        Q        R        S        T        U        V        W        X        Y        Z
    { nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, &LevelCritical_, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr }
};
static const LevelTreeNode_ LevelCritic_ =
{
    -1,
    //             A        B        C        D        E        F        G        H        I        J        K        L        M        N        O        P        Q        R        S        T        U        V        W        X        Y        Z
    { &LevelCritica_, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr }
};
static const LevelTreeNode_ LevelCriti_ =
{
    -1,
    //      A        B              C        D        E        F        G        H        I        J        K        L        M        N        O        P        Q        R        S        T        U        V        W        X        Y        Z
    { nullptr, nullptr, &LevelCritic_, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr }
};
static const LevelTreeNode_ LevelCrit_ =
{
    -1,
    //      A        B        C        D        E        F        G        H             I        J        K        L        M        N        O        P        Q        R        S        T        U        V        W        X        Y        Z
    { nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, &LevelCriti_, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr }
};
static const LevelTreeNode_ LevelCri_ =
{
    -1,
    //      A        B        C        D        E        F        G        H        I        J        K        L        M        N        O        P        Q        R        S            T        U        V        W        X        Y        Z
    { nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, &LevelCrit_, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr }
};
static const LevelTreeNode_ LevelCr_ =
{
    -1,
    //      A        B        C        D        E        F        G        H           I        J        K        L        M        N        O        P        Q        R        S        T        U        V        W        X        Y        Z
    { nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, &LevelCri_, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr }
};
static const LevelTreeNode_ LevelC_ =
{
    -1,
    //      A        B        C        D        E        F        G        H        I        J        K        L        M        N        O        P        Q          R        S        T        U        V        W        X        Y        Z
    { nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, &LevelCr_, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr }
};
static const LevelTreeNode_ LevelDebug_ =
{
    30000,
    //      A        B        C        D        E        F        G        H        I        J        K        L        M        N        O        P        Q        R        S        T        U        V        W        X        Y        Z
    { nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr }
};
static const LevelTreeNode_ LevelDebu_ =
{
    -1,
    //      A        B        C        D        E        F             G        H        I        J        K        L        M        N        O        P        Q        R        S        T        U        V        W        X        Y        Z
    { nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, &LevelDebug_, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr }
};
static const LevelTreeNode_ LevelDeb_ =
{
    -1,
    //      A        B        C        D        E        F        G        H        I        J        K        L        M        N        O        P        Q        R        S        T            U        V        W        X        Y        Z
    { nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, &LevelDebu_, nullptr, nullptr, nullptr, nullptr, nullptr }
};
static const LevelTreeNode_ LevelDe_ =
{
    -1,
    //      A           B        C        D        E        F        G        H        I        J        K        L        M        N        O        P        Q        R        S        T        U        V        W        X        Y        Z
    { nullptr, &LevelDeb_, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr }
};
static const LevelTreeNode_ LevelD_ =
{
    -1,
    //      A        B        C        D          E        F        G        H        I        J        K        L        M        N        O        P        Q        R        S        T        U        V        W        X        Y        Z
    { nullptr, nullptr, nullptr, nullptr, &LevelDe_, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr }
};
static const LevelTreeNode_ LevelError_ =
{
    70000,
    //      A        B        C        D        E        F        G        H        I        J        K        L        M        N        O        P        Q        R        S        T        U        V        W        X        Y        Z
    { nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr }
};
static const LevelTreeNode_ LevelErro_ =
{
    -1,
    //      A        B        C        D        E        F        G        H        I        J        K        L        M        N        O        P        Q             R        S        T        U        V        W        X        Y        Z
    { nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, &LevelError_, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr }
};
static const LevelTreeNode_ LevelErr_ =
{
    -1,
    //      A        B        C        D        E        F        G        H        I        J        K        L        M        N            O        P        Q        R        S        T        U        V        W        X        Y        Z
    { nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, &LevelErro_, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr }
};
static const LevelTreeNode_ LevelEr_ =
{
    -1,
    //      A        B        C        D        E        F        G        H        I        J        K        L        M        N        O        P        Q           R        S        T        U        V        W        X        Y        Z
    { nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, &LevelErr_, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr }
};
static const LevelTreeNode_ LevelE_ =
{
    -1,
    //      A        B        C        D        E        F        G        H        I        J        K        L        M        N        O        P        Q          R        S        T        U        V        W        X        Y        Z
    { nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, &LevelEr_, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr }
};
static const LevelTreeNode_ LevelFinest_ =
{
    10000,
    //      A        B        C        D        E        F        G        H        I        J        K        L        M        N        O        P        Q        R        S        T        U        V        W        X        Y        Z
    { nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr }
};
static const LevelTreeNode_ LevelFines_ =
{
    -1,
    //      A        B        C        D        E        F        G        H        I        J        K        L        M        N        O        P        Q        R        S              T        U        V        W        X        Y        Z
    { nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, &LevelFinest_, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr }
};
static const LevelTreeNode_ LevelFine_ =
{
    -1,
    //      A        B        C        D        E        F        G        H        I        J        K        L        M        N        O        P        Q        R             S        T        U        V        W        X        Y        Z
    { nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, &LevelFines_, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr }
};
static const LevelTreeNode_ LevelFin_ =
{
    -1,
    //      A        B        C        D            E        F        G        H        I        J        K        L        M        N        O        P        Q        R        S        T        U        V        W        X        Y        Z
    { nullptr, nullptr, nullptr, nullptr, &LevelFine_, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr }
};
static const LevelTreeNode_ LevelFi_ =
{
    -1,
    //      A        B        C        D        E        F        G        H        I        J        K        L        M           N        O        P        Q        R        S        T        U        V        W        X        Y        Z
    { nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, &LevelFin_, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr }
};
static const LevelTreeNode_ LevelF_ =
{
    -1,
    //      A        B        C        D        E        F        G        H          I        J        K        L        M        N        O        P        Q        R        S        T        U        V        W        X        Y        Z
    { nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, &LevelFi_, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr }
};
static const LevelTreeNode_ LevelInfo_ =
{
    40000,
    //      A        B        C        D        E        F        G        H        I        J        K        L        M        N        O        P        Q        R        S        T        U        V        W        X        Y        Z
    { nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr }
};
static const LevelTreeNode_ LevelInf_ =
{
    -1,
    //      A        B        C        D        E        F        G        H        I        J        K        L        M        N            O        P        Q        R        S        T        U        V        W        X        Y        Z
    { nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, &LevelInfo_, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr }
};
static const LevelTreeNode_ LevelIn_ =
{
    -1,
    //      A        B        C        D        E           F        G        H        I        J        K        L        M        N        O        P        Q        R        S        T        U        V        W        X        Y        Z
    { nullptr, nullptr, nullptr, nullptr, nullptr, &LevelInf_, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr }
};
static const LevelTreeNode_ LevelI_ =
{
    -1,
    //      A        B        C        D        E        F        G        H        I        J        K        L        M          N        O        P        Q        R        S        T        U        V        W        X        Y        Z
    { nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, &LevelIn_, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr }
};
static const LevelTreeNode_ LevelNotice_ =
{
    50000,
    //      A        B        C        D        E        F        G        H        I        J        K        L        M        N        O        P        Q        R        S        T        U        V        W        X        Y        Z
    { nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr }
};
static const LevelTreeNode_ LevelNotic_ =
{
    -1,
    //      A        B        C        D              E        F        G        H        I        J        K        L        M        N        O        P        Q        R        S        T        U        V        W        X        Y        Z
    { nullptr, nullptr, nullptr, nullptr, &LevelNotice_, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr }
};
static const LevelTreeNode_ LevelNoti_ =
{
    -1,
    //      A        B             C        D        E        F        G        H        I        J        K        L        M        N        O        P        Q        R        S        T        U        V        W        X        Y        Z
    { nullptr, nullptr, &LevelNotic_, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr }
};
static const LevelTreeNode_ LevelNot_ =
{
    -1,
    //      A        B        C        D        E        F        G        H            I        J        K        L        M        N        O        P        Q        R        S        T        U        V        W        X        Y        Z
    { nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, &LevelNoti_, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr }
};
static const LevelTreeNode_ LevelNo_ =
{
    -1,
    //      A        B        C        D        E        F        G        H        I        J        K        L        M        N        O        P        Q        R        S           T        U        V        W        X        Y        Z
    { nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, &LevelNot_, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr }
};
static const LevelTreeNode_ LevelN_ =
{
    -1,
    //      A        B        C        D        E        F        G        H        I        J        K        L        M        N          O        P        Q        R        S        T        U        V        W        X        Y        Z
    { nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, &LevelNo_, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr }
};
static const LevelTreeNode_ LevelOff_ =
{
    INT_MAX,
    //      A        B        C        D        E        F        G        H        I        J        K        L        M        N        O        P        Q        R        S        T        U        V        W        X        Y        Z
    { nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr }
};
static const LevelTreeNode_ LevelOf_ =
{
    -1,
    //      A        B        C        D        E           F        G        H        I        J        K        L        M        N        O        P        Q        R        S        T        U        V        W        X        Y        Z
    { nullptr, nullptr, nullptr, nullptr, nullptr, &LevelOff_, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr }
};
static const LevelTreeNode_ LevelO_ =
{
    -1,
    //      A        B        C        D        E          F        G        H        I        J        K        L        M        N        O        P        Q        R        S        T        U        V        W        X        Y        Z
    { nullptr, nullptr, nullptr, nullptr, nullptr, &LevelOf_, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr }
};
static const LevelTreeNode_ LevelSevere_ =
{
    80000,
    //      A        B        C        D        E        F        G        H        I        J        K        L        M        N        O        P        Q        R        S        T        U        V        W        X        Y        Z
    { nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr }
};
static const LevelTreeNode_ LevelSever_ =
{
    -1,
    //      A        B        C        D              E        F        G        H        I        J        K        L        M        N        O        P        Q        R        S        T        U        V        W        X        Y        Z
    { nullptr, nullptr, nullptr, nullptr, &LevelSevere_, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr }
};
static const LevelTreeNode_ LevelSeve_ =
{
    -1,
    //      A        B        C        D        E        F        G        H        I        J        K        L        M        N        O        P        Q             R        S        T        U        V        W        X        Y        Z
    { nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, &LevelSever_, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr }
};
static const LevelTreeNode_ LevelSev_ =
{
    -1,
    //      A        B        C        D            E        F        G        H        I        J        K        L        M        N        O        P        Q        R        S        T        U        V        W        X        Y        Z
    { nullptr, nullptr, nullptr, nullptr, &LevelSeve_, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr }
};
static const LevelTreeNode_ LevelSe_ =
{
    -1,
    //      A        B        C        D        E        F        G        H        I        J        K        L        M        N        O        P        Q        R        S        T        U           V        W        X        Y        Z
    { nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, &LevelSev_, nullptr, nullptr, nullptr, nullptr }
};
static const LevelTreeNode_ LevelS_ =
{
    -1,
    //      A        B        C        D          E        F        G        H        I        J        K        L        M        N        O        P        Q        R        S        T        U        V        W        X        Y        Z
    { nullptr, nullptr, nullptr, nullptr, &LevelSe_, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr }
};
static const LevelTreeNode_ LevelTrace_ =
{
    20000,
    //      A        B        C        D        E        F        G        H        I        J        K        L        M        N        O        P        Q        R        S        T        U        V        W        X        Y        Z
    { nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr }
};
static const LevelTreeNode_ LevelTrac_ =
{
    -1,
    //      A        B        C        D             E        F        G        H        I        J        K        L        M        N        O        P        Q        R        S        T        U        V        W        X        Y        Z
    { nullptr, nullptr, nullptr, nullptr, &LevelTrace_, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr }
};
static const LevelTreeNode_ LevelTra_ =
{
    -1,
    //      A        B            C        D        E        F        G        H        I        J        K        L        M        N        O        P        Q        R        S        T        U        V        W        X        Y        Z
    { nullptr, nullptr, &LevelTrac_, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr }
};
static const LevelTreeNode_ LevelTr_ =
{
    -1,
    //         A        B        C        D        E        F        G        H        I        J        K        L        M        N        O        P        Q        R        S        T        U        V        W        X        Y        Z
    { &LevelTra_, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr }
};
static const LevelTreeNode_ LevelT_ =
{
    -1,
    //      A        B        C        D        E        F        G        H        I        J        K        L        M        N        O        P        Q          R        S        T        U        V        W        X        Y        Z
    { nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, &LevelTr_, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr }
};
static const LevelTreeNode_ LevelVerbose_ =
{
    10000,
    //      A        B        C        D        E        F        G        H        I        J        K        L        M        N        O        P        Q        R        S        T        U        V        W        X        Y        Z
    { nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr }
};
static const LevelTreeNode_ LevelVerbos_ =
{
    -1,
    //      A        B        C        D               E        F        G        H        I        J        K        L        M        N        O        P        Q        R        S        T        U        V        W        X        Y        Z
    { nullptr, nullptr, nullptr, nullptr, &LevelVerbose_, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr }
};
static const LevelTreeNode_ LevelVerbo_ =
{
    -1,
    //      A        B        C        D        E        F        G        H        I        J        K        L        M        N        O        P        Q        R              S        T        U        V        W        X        Y        Z
    { nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, &LevelVerbos_, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr }
};
static const LevelTreeNode_ LevelVerb_ =
{
    -1,
    //      A        B        C        D        E        F        G        H        I        J        K        L        M        N             O        P        Q        R        S        T        U        V        W        X        Y        Z
    { nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, &LevelVerbo_, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr }
};
static const LevelTreeNode_ LevelVer_ =
{
    -1,
    //      A            B        C        D        E        F        G        H        I        J        K        L        M        N        O        P        Q        R        S        T        U        V        W        X        Y        Z
    { nullptr, &LevelVerb_, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr }
};
static const LevelTreeNode_ LevelVe_ =
{
    -1,
    //      A        B        C        D        E        F        G        H        I        J        K        L        M        N        O        P        Q           R        S        T        U        V        W        X        Y        Z
    { nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, &LevelVer_, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr }
};
static const LevelTreeNode_ LevelV_ =
{
    -1,
    //      A        B        C        D          E        F        G        H        I        J        K        L        M        N        O        P        Q        R        S        T        U        V        W        X        Y        Z
    { nullptr, nullptr, nullptr, nullptr, &LevelVe_, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr }
};
static const LevelTreeNode_ LevelWarn_ =
{
    60000,
    //      A        B        C        D        E        F        G        H        I        J        K        L        M        N        O        P        Q        R        S        T        U        V        W        X        Y        Z
    { nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr }
};
static const LevelTreeNode_ LevelWar_ =
{
    -1,
    //      A        B        C        D        E        F        G        H        I        J        K        L        M            N        O        P        Q        R        S        T        U        V        W        X        Y        Z
    { nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, &LevelWarn_, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr }
};
static const LevelTreeNode_ LevelWa_ =
{
    -1,
    //      A        B        C        D        E        F        G        H        I        J        K        L        M        N        O        P        Q           R        S        T        U        V        W        X        Y        Z
    { nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, &LevelWar_, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr }
};
static const LevelTreeNode_ LevelW_ =
{
    -1,
    //        A        B        C        D        E        F        G        H        I        J        K        L        M        N        O        P        Q        R        S        T        U        V        W        X        Y        Z
    { &LevelWa_, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr, nullptr }
};
static const LevelTreeNode_ Levels_ =
{
    -1,
    //       A        B         C         D         E         F        G        H         I        J        K        L        M         N         O        P        Q        R         S         T        U         V         W        X        Y        Z
    { &LevelA_, nullptr, &LevelC_, &LevelD_, &LevelE_, &LevelF_, nullptr, nullptr, &LevelI_, nullptr, nullptr, nullptr, nullptr, &LevelN_, &LevelO_, nullptr, nullptr, nullptr, &LevelS_, &LevelT_, nullptr, &LevelV_, &LevelW_, nullptr, nullptr, nullptr }
};

#pragma endregion

int GetLevelValue (const char *level, size_t levelSize)
{
    return LtnGet_ (&Levels_, level, levelSize);
}
