export interface PhotoDetailed {
    id: number;
    name: string;
    originalLocation: string;
    processingAnalysisResult: {
        description: {
            captions: Caption[],
            tags: object[],
        }
    };
}

export interface Caption {
    text: string
}
