'use client'

import { Button } from "@/components/ui/button"

export default function GlobalError({
    error,
    reset,
}: {
    error: Error & { digest?: string }
    reset: () => void
}) {
    return (
        <html>
            <body>
                <div className="flex flex-col items-center justify-center min-h-screen bg-background text-foreground space-y-4 text-center px-4">
                    <h1 className="text-9xl font-bold text-destructive tracking-tighter">Crit</h1>
                    <h2 className="text-2xl font-semibold tracking-tight">Critical Error</h2>
                    <p className="text-muted-foreground max-w-[500px]">
                        A critical error occurred in the application layout. please refresh or contact support.
                    </p>
                    <div className="pt-4">
                        <Button
                            variant="default"
                            size="lg"
                            onClick={() => reset()}
                        >
                            Try again
                        </Button>
                    </div>
                </div>
            </body>
        </html>
    )
}
