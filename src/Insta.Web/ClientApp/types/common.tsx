export interface Result<TContent> {
    isSuccess: boolean;
    messsage: string;
    content: TContent;
}
