module SharedTypes

type SecurityToken = SecurityToken of string

type LoginInfo = { Username: string; Password: string }

// possible errors when logging in
type LoginError = 
    | UserDoesNotExist
    | PasswordIncorrect
    | AccountBanned

type BookId = BookId of int
// domain model
type Book = { Id: BookId; Title: string; (* other propeties *) }

// things that could happen when requesting to remove a book
type BookRemovalResult = 
    | BookSuccessfullyRemoved
    | BookDoesNotExist

// the book store protocol
type IBookStoreApi =
    {
        // login to acquire an auth token   
        login : LoginInfo -> Async<Result<SecurityToken, LoginError>>
        // "public" function: no auth needed
        searchBooksByTitle : string -> Async<list<Book>> 
        // secure function, requires a token
        booksOnWishlist : SecurityToken -> Async<Result<list<Book>, LoginError>> 
  
    }

