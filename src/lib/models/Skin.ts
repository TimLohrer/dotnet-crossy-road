export interface Skin {
    id: number;
    modelName: string;
    name: string;
    price: number;
    rarity: SkinRarity;
}

export enum SkinRarity {
    Common = 0,
    Rare = 1,
    Epic = 2,
    Legendary = 3
}