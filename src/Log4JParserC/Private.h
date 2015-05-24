#pragma once

#define __STR2__(x) #x
#define __STR1__(x) __STR2__(x)
// Allows to emit TODO messages as warnings on compilation with #pragma message:
// #pragma message (__TODO__ "Complete some task.")
#define __TODO__ __FILE__ "("__STR1__(__LINE__)") : Warning TODO: "
