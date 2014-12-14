#include "LevelParser.h"
#include <limits.h>

#define LTN_CHILD_COUNT 26

typedef struct LevelTreeNode_
{
    const int Value;
    const struct LevelTreeNode_ *Children[LTN_CHILD_COUNT];
} LevelTreeNode_;

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
    //   A     B     C     D     E     F     G     H     I     J     K     L     M     N     O     P     Q     R     S     T     U     V     W     X     Y     Z
    { NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL }
};
static const LevelTreeNode_ LevelAl_ =
{
    -1,
    //   A     B     C     D     E     F     G     H     I     J     K           L     M     N     O     P     Q     R     S     T     U     V     W     X     Y     Z
    { NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, &LevelAll_, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL }
};
static const LevelTreeNode_ LevelA_ =
{
    -1,
    //   A     B     C     D     E     F     G     H     I     J     K          L     M     N     O     P     Q     R     S     T     U     V     W     X     Y     Z
    { NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, &LevelAl_, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL }
};
static const LevelTreeNode_ LevelCritical_ =
{
    90000,
    //   A     B     C     D     E     F     G     H     I     J     K     L     M     N     O     P     Q     R     S     T     U     V     W     X     Y     Z
    { NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL }
};
static const LevelTreeNode_ LevelCritica_ =
{
    -1,
    //   A     B     C     D     E     F     G     H     I     J     K                L     M     N     O     P     Q     R     S     T     U     V     W     X     Y     Z
    { NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, &LevelCritical_, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL }
};
static const LevelTreeNode_ LevelCritic_ =
{
    -1,
    //             A     B     C     D     E     F     G     H     I     J     K     L     M     N     O     P     Q     R     S     T     U     V     W     X     Y     Z
    { &LevelCritica_, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL }
};
static const LevelTreeNode_ LevelCriti_ =
{
    -1,
    //   A     B              C     D     E     F     G     H     I     J     K     L     M     N     O     P     Q     R     S     T     U     V     W     X     Y     Z
    { NULL, NULL, &LevelCritic_, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL }
};
static const LevelTreeNode_ LevelCrit_ =
{
    -1,
    //   A     B     C     D     E     F     G     H             I     J     K     L     M     N     O     P     Q     R     S     T     U     V     W     X     Y     Z
    { NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, &LevelCriti_, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL }
};
static const LevelTreeNode_ LevelCri_ =
{
    -1,
    //   A     B     C     D     E     F     G     H     I     J     K     L     M     N     O     P     Q     R     S            T     U     V     W     X     Y     Z
    { NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, &LevelCrit_, NULL, NULL, NULL, NULL, NULL, NULL }
};
static const LevelTreeNode_ LevelCr_ =
{
    -1,
    //   A     B     C     D     E     F     G     H           I     J     K     L     M     N     O     P     Q     R     S     T     U     V     W     X     Y     Z
    { NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, &LevelCri_, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL }
};
static const LevelTreeNode_ LevelC_ =
{
    -1,
    //   A     B     C     D     E     F     G     H     I     J     K     L     M     N     O     P     Q          R     S     T     U     V     W     X     Y     Z
    { NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, &LevelCr_, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL }
};
static const LevelTreeNode_ LevelDebug_ =
{
    30000,
    //   A     B     C     D     E     F     G     H     I     J     K     L     M     N     O     P     Q     R     S     T     U     V     W     X     Y     Z
    { NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL }
};
static const LevelTreeNode_ LevelDebu_ =
{
    -1,
    //   A     B     C     D     E     F             G     H     I     J     K     L     M     N     O     P     Q     R     S     T     U     V     W     X     Y     Z
    { NULL, NULL, NULL, NULL, NULL, NULL, &LevelDebug_, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL }
};
static const LevelTreeNode_ LevelDeb_ =
{
    -1,
    //   A     B     C     D     E     F     G     H     I     J     K     L     M     N     O     P     Q     R     S     T            U     V     W     X     Y     Z
    { NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, &LevelDebu_, NULL, NULL, NULL, NULL, NULL }
};
static const LevelTreeNode_ LevelDe_ =
{
    -1,
    //   A           B     C     D     E     F     G     H     I     J     K     L     M     N     O     P     Q     R     S     T     U     V     W     X     Y     Z
    { NULL, &LevelDeb_, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL }
};
static const LevelTreeNode_ LevelD_ =
{
    -1,
    //   A     B     C     D          E     F     G     H     I     J     K     L     M     N     O     P     Q     R     S     T     U     V     W     X     Y     Z
    { NULL, NULL, NULL, NULL, &LevelDe_, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL }
};
static const LevelTreeNode_ LevelError_ =
{
    70000,
    //   A     B     C     D     E     F     G     H     I     J     K     L     M     N     O     P     Q     R     S     T     U     V     W     X     Y     Z
    { NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL }
};
static const LevelTreeNode_ LevelErro_ =
{
    -1,
    //   A     B     C     D     E     F     G     H     I     J     K     L     M     N     O     P     Q             R     S     T     U     V     W     X     Y     Z
    { NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, &LevelError_, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL }
};
static const LevelTreeNode_ LevelErr_ =
{
    -1,
    //   A     B     C     D     E     F     G     H     I     J     K     L     M     N            O     P     Q     R     S     T     U     V     W     X     Y     Z
    { NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, &LevelErro_, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL }
};
static const LevelTreeNode_ LevelEr_ =
{
    -1,
    //   A     B     C     D     E     F     G     H     I     J     K     L     M     N     O     P     Q           R     S     T     U     V     W     X     Y     Z
    { NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, &LevelErr_, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL }
};
static const LevelTreeNode_ LevelE_ =
{
    -1,
    //   A     B     C     D     E     F     G     H     I     J     K     L     M     N     O     P     Q          R     S     T     U     V     W     X     Y     Z
    { NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, &LevelEr_, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL }
};
static const LevelTreeNode_ LevelFinest_ =
{
    10000,
    //   A     B     C     D     E     F     G     H     I     J     K     L     M     N     O     P     Q     R     S     T     U     V     W     X     Y     Z
    { NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL }
};
static const LevelTreeNode_ LevelFines_ =
{
    -1,
    //   A     B     C     D     E     F     G     H     I     J     K     L     M     N     O     P     Q     R     S              T     U     V     W     X     Y     Z
    { NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, &LevelFinest_, NULL, NULL, NULL, NULL, NULL, NULL }
};
static const LevelTreeNode_ LevelFine_ =
{
    -1,
    //   A     B     C     D     E     F     G     H     I     J     K     L     M     N     O     P     Q     R             S     T     U     V     W     X     Y     Z
    { NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, &LevelFines_, NULL, NULL, NULL, NULL, NULL, NULL, NULL }
};
static const LevelTreeNode_ LevelFin_ =
{
    -1,
    //   A     B     C     D            E     F     G     H     I     J     K     L     M     N     O     P     Q     R     S     T     U     V     W     X     Y     Z
    { NULL, NULL, NULL, NULL, &LevelFine_, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL }
};
static const LevelTreeNode_ LevelFi_ =
{
    -1,
    //   A     B     C     D     E     F     G     H     I     J     K     L     M           N     O     P     Q     R     S     T     U     V     W     X     Y     Z
    { NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, &LevelFin_, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL }
};
static const LevelTreeNode_ LevelF_ =
{
    -1,
    //   A     B     C     D     E     F     G     H          I     J     K     L     M     N     O     P     Q     R     S     T     U     V     W     X     Y     Z
    { NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, &LevelFi_, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL }
};
static const LevelTreeNode_ LevelInfo_ =
{
    40000,
    //   A     B     C     D     E     F     G     H     I     J     K     L     M     N     O     P     Q     R     S     T     U     V     W     X     Y     Z
    { NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL }
};
static const LevelTreeNode_ LevelInf_ =
{
    -1,
    //   A     B     C     D     E     F     G     H     I     J     K     L     M     N            O     P     Q     R     S     T     U     V     W     X     Y     Z
    { NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, &LevelInfo_, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL }
};
static const LevelTreeNode_ LevelIn_ =
{
    -1,
    //   A     B     C     D     E           F     G     H     I     J     K     L     M     N     O     P     Q     R     S     T     U     V     W     X     Y     Z
    { NULL, NULL, NULL, NULL, NULL, &LevelInf_, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL }
};
static const LevelTreeNode_ LevelI_ =
{
    -1,
    //   A     B     C     D     E     F     G     H     I     J     K     L     M          N     O     P     Q     R     S     T     U     V     W     X     Y     Z
    { NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, &LevelIn_, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL }
};
static const LevelTreeNode_ LevelNotice_ =
{
    50000,
    //   A     B     C     D     E     F     G     H     I     J     K     L     M     N     O     P     Q     R     S     T     U     V     W     X     Y     Z
    { NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL }
};
static const LevelTreeNode_ LevelNotic_ =
{
    -1,
    //   A     B     C     D              E     F     G     H     I     J     K     L     M     N     O     P     Q     R     S     T     U     V     W     X     Y     Z
    { NULL, NULL, NULL, NULL, &LevelNotice_, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL }
};
static const LevelTreeNode_ LevelNoti_ =
{
    -1,
    //   A     B             C     D     E     F     G     H     I     J     K     L     M     N     O     P     Q     R     S     T     U     V     W     X     Y     Z
    { NULL, NULL, &LevelNotic_, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL }
};
static const LevelTreeNode_ LevelNot_ =
{
    -1,
    //   A     B     C     D     E     F     G     H            I     J     K     L     M     N     O     P     Q     R     S     T     U     V     W     X     Y     Z
    { NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, &LevelNoti_, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL }
};
static const LevelTreeNode_ LevelNo_ =
{
    -1,
    //   A     B     C     D     E     F     G     H     I     J     K     L     M     N     O     P     Q     R     S           T     U     V     W     X     Y     Z
    { NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, &LevelNot_, NULL, NULL, NULL, NULL, NULL, NULL }
};
static const LevelTreeNode_ LevelN_ =
{
    -1,
    //   A     B     C     D     E     F     G     H     I     J     K     L     M     N          O     P     Q     R     S     T     U     V     W     X     Y     Z
    { NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, &LevelNo_, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL }
};
static const LevelTreeNode_ LevelOff_ =
{
    INT_MAX,
    //   A     B     C     D     E     F     G     H     I     J     K     L     M     N     O     P     Q     R     S     T     U     V     W     X     Y     Z
    { NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL }
};
static const LevelTreeNode_ LevelOf_ =
{
    -1,
    //   A     B     C     D     E           F     G     H     I     J     K     L     M     N     O     P     Q     R     S     T     U     V     W     X     Y     Z
    { NULL, NULL, NULL, NULL, NULL, &LevelOff_, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL }
};
static const LevelTreeNode_ LevelO_ =
{
    -1,
    //   A     B     C     D     E          F     G     H     I     J     K     L     M     N     O     P     Q     R     S     T     U     V     W     X     Y     Z
    { NULL, NULL, NULL, NULL, NULL, &LevelOf_, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL }
};
static const LevelTreeNode_ LevelSevere_ =
{
    80000,
    //   A     B     C     D     E     F     G     H     I     J     K     L     M     N     O     P     Q     R     S     T     U     V     W     X     Y     Z
    { NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL }
};
static const LevelTreeNode_ LevelSever_ =
{
    -1,
    //   A     B     C     D              E     F     G     H     I     J     K     L     M     N     O     P     Q     R     S     T     U     V     W     X     Y     Z
    { NULL, NULL, NULL, NULL, &LevelSevere_, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL }
};
static const LevelTreeNode_ LevelSeve_ =
{
    -1,
    //   A     B     C     D     E     F     G     H     I     J     K     L     M     N     O     P     Q             R     S     T     U     V     W     X     Y     Z
    { NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, &LevelSever_, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL }
};
static const LevelTreeNode_ LevelSev_ =
{
    -1,
    //   A     B     C     D            E     F     G     H     I     J     K     L     M     N     O     P     Q     R     S     T     U     V     W     X     Y     Z
    { NULL, NULL, NULL, NULL, &LevelSeve_, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL }
};
static const LevelTreeNode_ LevelSe_ =
{
    -1,
    //   A     B     C     D     E     F     G     H     I     J     K     L     M     N     O     P     Q     R     S     T     U           V     W     X     Y     Z
    { NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, &LevelSev_, NULL, NULL, NULL, NULL }
};
static const LevelTreeNode_ LevelS_ =
{
    -1,
    //   A     B     C     D          E     F     G     H     I     J     K     L     M     N     O     P     Q     R     S     T     U     V     W     X     Y     Z
    { NULL, NULL, NULL, NULL, &LevelSe_, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL }
};
static const LevelTreeNode_ LevelTrace_ =
{
    20000,
    //   A     B     C     D     E     F     G     H     I     J     K     L     M     N     O     P     Q     R     S     T     U     V     W     X     Y     Z
    { NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL }
};
static const LevelTreeNode_ LevelTrac_ =
{
    -1,
    //   A     B     C     D             E     F     G     H     I     J     K     L     M     N     O     P     Q     R     S     T     U     V     W     X     Y     Z
    { NULL, NULL, NULL, NULL, &LevelTrace_, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL }
};
static const LevelTreeNode_ LevelTra_ =
{
    -1,
    //   A     B            C     D     E     F     G     H     I     J     K     L     M     N     O     P     Q     R     S     T     U     V     W     X     Y     Z
    { NULL, NULL, &LevelTrac_, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL }
};
static const LevelTreeNode_ LevelTr_ =
{
    -1,
    //         A     B     C     D     E     F     G     H     I     J     K     L     M     N     O     P     Q     R     S     T     U     V     W     X     Y     Z
    { &LevelTra_, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL }
};
static const LevelTreeNode_ LevelT_ =
{
    -1,
    //   A     B     C     D     E     F     G     H     I     J     K     L     M     N     O     P     Q          R     S     T     U     V     W     X     Y     Z
    { NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, &LevelTr_, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL }
};
static const LevelTreeNode_ LevelVerbose_ =
{
    10000,
    //   A     B     C     D     E     F     G     H     I     J     K     L     M     N     O     P     Q     R     S     T     U     V     W     X     Y     Z
    { NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL }
};
static const LevelTreeNode_ LevelVerbos_ =
{
    -1,
    //   A     B     C     D               E     F     G     H     I     J     K     L     M     N     O     P     Q     R     S     T     U     V     W     X     Y     Z
    { NULL, NULL, NULL, NULL, &LevelVerbose_, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL }
};
static const LevelTreeNode_ LevelVerbo_ =
{
    -1,
    //   A     B     C     D     E     F     G     H     I     J     K     L     M     N     O     P     Q     R              S     T     U     V     W     X     Y     Z
    { NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, &LevelVerbos_, NULL, NULL, NULL, NULL, NULL, NULL, NULL }
};
static const LevelTreeNode_ LevelVerb_ =
{
    -1,
    //   A     B     C     D     E     F     G     H     I     J     K     L     M     N             O     P     Q     R     S     T     U     V     W     X     Y     Z
    { NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, &LevelVerbo_, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL }
};
static const LevelTreeNode_ LevelVer_ =
{
    -1,
    //   A            B     C     D     E     F     G     H     I     J     K     L     M     N     O     P     Q     R     S     T     U     V     W     X     Y     Z
    { NULL, &LevelVerb_, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL }
};
static const LevelTreeNode_ LevelVe_ =
{
    -1,
    //   A     B     C     D     E     F     G     H     I     J     K     L     M     N     O     P     Q           R     S     T     U     V     W     X     Y     Z
    { NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, &LevelVer_, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL }
};
static const LevelTreeNode_ LevelV_ =
{
    -1,
    //   A     B     C     D          E     F     G     H     I     J     K     L     M     N     O     P     Q     R     S     T     U     V     W     X     Y     Z
    { NULL, NULL, NULL, NULL, &LevelVe_, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL }
};
static const LevelTreeNode_ LevelWarn_ =
{
    60000,
    //   A     B     C     D     E     F     G     H     I     J     K     L     M     N     O     P     Q     R     S     T     U     V     W     X     Y     Z
    { NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL }
};
static const LevelTreeNode_ LevelWar_ =
{
    -1,
    //   A     B     C     D     E     F     G     H     I     J     K     L     M            N     O     P     Q     R     S     T     U     V     W     X     Y     Z
    { NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, &LevelWarn_, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL }
};
static const LevelTreeNode_ LevelWa_ =
{
    -1,
    //   A     B     C     D     E     F     G     H     I     J     K     L     M     N     O     P     Q           R     S     T     U     V     W     X     Y     Z
    { NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, &LevelWar_, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL }
};
static const LevelTreeNode_ LevelW_ =
{
    -1,
    //        A     B     C     D     E     F     G     H     I     J     K     L     M     N     O     P     Q     R     S     T     U     V     W     X     Y     Z
    { &LevelWa_, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL }
};
static const LevelTreeNode_ Levels_ =
{
    -1,
    //       A     B         C         D         E         F     G     H         I     J     K     L     M         N         O     P     Q     R         S         T     U         V         W     X     Y     Z
    { &LevelA_, NULL, &LevelC_, &LevelD_, &LevelE_, &LevelF_, NULL, NULL, &LevelI_, NULL, NULL, NULL, NULL, &LevelN_, &LevelO_, NULL, NULL, NULL, &LevelS_, &LevelT_, NULL, &LevelV_, &LevelW_, NULL, NULL, NULL }
};

#pragma endregion

int GetLevelValue (const char *level, size_t levelSize)
{
    return LtnGet_ (&Levels_, level, levelSize);
}
