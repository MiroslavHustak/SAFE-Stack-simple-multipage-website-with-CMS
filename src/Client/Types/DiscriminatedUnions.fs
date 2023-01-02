namespace DiscriminatedUnions.Client

type Deferred<'t> =
    | HasNotStartedYet
    | InProgress
    | Resolved of 't

