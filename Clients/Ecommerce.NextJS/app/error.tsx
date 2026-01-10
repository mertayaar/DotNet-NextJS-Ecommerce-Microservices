'use client'

import { useEffect } from 'react'
import Link from 'next/link'
import { Button } from "@/components/ui/button"

export default function Error({
    error,
    reset,
}: {
    error: Error & { digest?: string }
    reset: () => void
}) {
    useEffect(() => {
        
        console.error(error)
    }, [error])

    return (
        <div className="flex flex-col items-center justify-center min-h-screen bg-background text-foreground space-y-4 text-center px-4">
            <h1 className="text-9xl font-bold text-destructive tracking-tighter">500</h1>
            <h2 className="text-2xl font-semibold tracking-tight">Something went wrong!</h2>
            <p className="text-muted-foreground max-w-[500px]">
                We apologize for the inconvenience. An unexpected error has occurred.
            </p>
            <div className="flex gap-4 pt-4">
                <Button
                    variant="outline"
                    size="lg"
                    onClick={() => reset()}
                >
                    Try again
                </Button>
                <Link href="/">
                    <Button variant="default" size="lg">
                        Return Home
                    </Button>
                </Link>
            </div>
        </div>
    )
}
