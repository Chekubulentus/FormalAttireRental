export class Result<T> {
    private constructor(
        public readonly isSuccess: boolean,
        public readonly data ?: T,
        public readonly errorMessage ?: string,
        public readonly successMessage ?: string
    ) {}

    public static success<T>(data: T) : Result<T> {
        return new Result<T>(true, data, undefined, undefined);
    }

    public static successWithMessage<T>(message : string) : Result<T> {
        return new Result<T>(true, undefined, undefined, message);
    }

    public static failure<T>(message: string) : Result<T> {
        return new Result<T>(false, undefined, message, undefined);
    }
}