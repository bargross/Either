# Either

A Control flow data type, a monadic type taken from the functional paradigm, specifically Haskell to allow OO languages (C#) 
to overcome the limitations of the paradigm, in this case the limitation is the fact that functions or methods in OO languages
allow you to return a single type. 

Polymorphism or as is known in OO languages, allows languages under this paradigm to overcome that limitation (depending on the language) 
to a certain degree. 

Either overcomes that limitation by allowing you to set the type to a predefined generic types. Currently you can only define 2 types
and the value marked as type Either can be set to either of these types, but only 1, it can never be assigned to contain more than 1 type.
