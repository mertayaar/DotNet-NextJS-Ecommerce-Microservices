import Image from "next/image"

export default function AboutPage() {
    return (
        <div className="container py-12 md:py-24">
            <div className="grid gap-12 lg:grid-cols-2 items-center">
                <div className="relative aspect-square overflow-hidden rounded-xl bg-muted">
                    <Image
                        src="https://images.unsplash.com/photo-1441986300917-64674bd600d8?auto=format&fit=crop&q=80&w=800"
                        alt="Our Workshop"
                        fill
                        className="object-cover"
                    />
                </div>
                <div className="flex flex-col gap-6">
                    <h1 className="text-4xl font-bold tracking-tight">About Minimal Store</h1>
                    <p className="text-lg text-muted-foreground">
                        We believe in the power of simplicity. Established in 2024, Minimal Store was born from a desire to strip away the noise and focus on what truly matters: quality, sustainability, and timeless design.
                    </p>
                    <div className="grid gap-4 sm:grid-cols-2">
                        <div className="rounded-lg border p-4">
                            <h3 className="font-semibold mb-2">Sustainable</h3>
                            <p className="text-sm text-muted-foreground">Ethically sourced materials and responsible manufacturing.</p>
                        </div>
                        <div className="rounded-lg border p-4">
                            <h3 className="font-semibold mb-2">Timeless</h3>
                            <p className="text-sm text-muted-foreground">Designs that transcend seasons and trends.</p>
                        </div>
                    </div>
                    <p className="text-muted-foreground">
                        Our collection is curated for the modern individual who values aesthetics and functionality in equal measure. Every piece tells a story of craftsmanship and dedication to detail.
                    </p>
                </div>

            </div>
        </div>
    )
}
