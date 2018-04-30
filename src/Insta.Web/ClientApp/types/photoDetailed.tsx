export interface FaceRectangle {
    top: number,
    left: number,
    width: number,
    height: number,
}

interface Face {
    age: number,
    gender: string,
    faceRectangle: FaceRectangle,
}

interface Caption {
    text: string,
    confidence: string,
}

interface Description {
    captions: Caption[],
    tags: object[],
}

export interface ProcessingAnalysisResult {
    description: Description,
    faces: Face[],
}

export interface PhotoDetailed {
    id: number;
    name: string;
    originalLocation: string;
    processingAnalysisResult: ProcessingAnalysisResult;
}
